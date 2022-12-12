using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

public class CLRTreeView : TreeView
{
    private PeInfo info;
    private TableStream tableStream;
    private StringBuilder stringBuilder = new StringBuilder();
    private BytesArray reader;
    private RawMethodRow currMethodRow;

    public void Init(PeInfo info)
    {
        this.info = info;
        reader = info.bytes;
        tableStream = MetadataHeader.tableStream;
    }

    public unsafe RawTypeDefRow ExcuteClass(uint index, TabControl tabControl)
    {
        this.Nodes.Clear();
        var typeDef = tableStream.listTypeDefMD[index];
        TreeNode clrNode;
        foreach (RawMethodRow row in typeDef.methodRidList)
        {
            var rid = row.Rid;
            this.currMethodRow = row;
            stringBuilder.Clear();
            stringBuilder.Append(rid.ToString());
            stringBuilder.Append(" : ");
            stringBuilder.Append(MetadataHeader.StringHeapStream.Read(row.Name));
            clrNode = AddClrNode(stringBuilder.ToString());

            reader.Position = info.Rva2Fov(row.RVA);
            byte b = reader.ReadInt8();
            uint codeSize = 0;
            uint flags;
            uint maxStack;
            uint localVarSigTok;
            uint headerSize;
            switch (b & 7)
            {
                case 2:
                case 6:
                    // Tiny header. [7:2] = code size, max stack is 8, no locals or exception handlers
                    flags = 2;
                    maxStack = 8;
                    codeSize = (uint)(b >> 2);
                    localVarSigTok = 0;
                    headerSize = 1;
                    break;

                case 3:
                    // Fat header. Can have locals and exception handlers
                    flags = (ushort)((reader.ReadInt8() << 8) | b);
                    headerSize = (byte)(flags >> 12);
                    maxStack = reader.ReadUInt16();
                    codeSize = reader.ReadUInt32();
                    localVarSigTok = reader.ReadUInt32();
                    // The CLR allows the code to start inside the method header. But if it does,
                    // the CLR doesn't read any exceptions.
                    reader.Position = reader.Position - 12 + (uint)headerSize * 4U;
                    if (headerSize < 3)
                        flags &= 0xFFF7;
                    headerSize *= 4;
                    break;
                default:
                    Debug.Assert(false);
                    break;
            }

            byte* ip = reader.GetPtr();
            byte* startIp = ip;
            byte* codeEnd = ip + codeSize;
            while (ip < codeEnd)
            {
                OpCodeInfo oc = OpCodeInfo.DecodeOpCodeInfo(&ip, codeEnd);
                int opCodeSize = OpCodeInfo.GetOpCodeSize(ip, oc);
                byte* nextIp = ip + opCodeSize;
                oc.Offset = (uint)(ip - startIp);
                oc.ip = ip + 1;
                oc.OpCodeSize = opCodeSize;
                string param = ReadOperand(oc) + "";
                stringBuilder.Clear();
                stringBuilder.Append(ToIlPtrBase(oc.Offset));
                stringBuilder.Append(": ");
                AddILNameSpace(oc.name, 15);
                stringBuilder.Append(param);
                AddClrNode(stringBuilder.ToString(), clrNode);
                ip = nextIp;
            }

            clrNode.ExpandAll();
        }

        return typeDef;
    }


    object ReadOperand(OpCodeInfo instr) =>
        instr.operandType switch
        {
            OperandType.InlineBrTarget => ReadInlineBrTarget(instr),
            OperandType.InlineField => ReadInlineField(instr),
            OperandType.InlineI => ReadInlineI(instr),
            OperandType.InlineI8 => ReadInlineI8(instr),
            OperandType.InlineMethod => ReadInlineMethod(instr),
            OperandType.InlineNone => ReadInlineNone(instr),
            OperandType.InlinePhi => ReadInlinePhi(instr),
            OperandType.InlineR => ReadInlineR(instr),
            OperandType.InlineSig => ReadInlineSig(instr),
            OperandType.InlineString => ReadInlineString(instr),
            OperandType.InlineSwitch => ReadInlineSwitch(instr),
            OperandType.InlineTok => ReadInlineTok(instr),
            OperandType.InlineType => ReadInlineType(instr),
            OperandType.InlineVar => ReadInlineVar(instr),
            OperandType.ShortInlineBrTarget => ReadShortInlineBrTarget(instr),
            OperandType.ShortInlineI => ReadShortInlineI(instr),
            OperandType.ShortInlineR => ReadShortInlineR(instr),
            OperandType.ShortInlineVar => ReadShortInlineVar(instr),
            _ => throw new InvalidOperationException("Invalid OpCode.OperandType"),
        };

    protected virtual string ReadInlineR(OpCodeInfo instr) => "ReadInlineR没实现";
    protected virtual string ReadInlineSig(OpCodeInfo instr)
    {
        MetadataHeader.tableStream.ReadTypeSignature(instr.ReadUInt32());
        return "ReadInlineSig没实现";
    }

    protected virtual string ReadInlineVar(OpCodeInfo instr) => "ReadInlineVar没实现";
    protected virtual string ReadInlineSwitch(OpCodeInfo instr) => "ReadInlineSwitch没实现";
    protected virtual string ReadInlinePhi(OpCodeInfo instr) => "<*****ReadInlinePhi*****>";
    
    protected virtual object ReadShortInlineI(OpCodeInfo instr)
    {
        if (instr.id == OpcodeEnum.LDC_I4_S)
            return instr.ReadSInt8();
        return instr.ReadInt8();
    }

    protected virtual float ReadShortInlineR(OpCodeInfo instr) => instr.ReadSingle();

    protected virtual uint ReadInlineBrTarget(OpCodeInfo instr) =>
        instr.Offset + (uint)instr.OpCodeSize + instr.ReadUInt32();

    protected unsafe string ReadShortInlineBrTarget(OpCodeInfo instr) =>
        ToIlPtrBase(instr.Offset + (uint)instr.OpCodeSize + (uint)instr.ReadSInt8());

    protected virtual int ReadInlineI(OpCodeInfo instr) => instr.ReadInt32();
    protected virtual long ReadInlineI8(OpCodeInfo instr) => instr.ReadInt64();

    protected virtual string ReadShortInlineVar(OpCodeInfo instr)
    {
        if (IsArgOperandInstruction(instr))
            return ReadShortInlineVarArg(instr);
        return ReadShortInlineVarLocal(instr);
    }

    protected virtual string ReadShortInlineVarArg(OpCodeInfo instr)
    {
        var index = instr.ReadInt8();
        if (index >= currMethodRow.paramRows.Count)
            return "Null";
        var param = currMethodRow.paramRows[index];
        return param.ToString();
    }

    protected virtual string ReadShortInlineVarLocal(OpCodeInfo instr)
    {
        var index = instr.ReadInt8();
        return "V_" + index;
    }

    protected static bool IsArgOperandInstruction(OpCodeInfo instr)
    {
        switch (instr.id)
        {
            case OpcodeEnum.LDARG:
            case OpcodeEnum.LDARG_S:
            case OpcodeEnum.LDARGA:
            case OpcodeEnum.LDARGA_S:
            case OpcodeEnum.STARG:
            case OpcodeEnum.STARG_S:
                return true;
            default:
                return false;
        }
    }

    protected virtual string ReadInlineNone(OpCodeInfo instr) => "";

    protected virtual string ReadInlineField(OpCodeInfo instr) =>
        MetadataHeader.tableStream.ResolveToken(instr.ReadUInt32());

    protected virtual string ReadInlineMethod(OpCodeInfo instr) => MetadataHeader.tableStream.ResolveToken(instr.ReadUInt32());

    protected virtual string ReadInlineString(OpCodeInfo instr) =>
        "\"" + PETools.GetHexString(MetadataHeader.UsHeapStream.ReadUTF16String(instr.ReadUInt32() & 0x00FFFFFF), "UNICODE") +
        "\"";


    protected virtual string ReadInlineTok(OpCodeInfo instr) => MetadataHeader.tableStream.ResolveToken(instr.ReadUInt32());

    protected virtual string ReadInlineType(OpCodeInfo instr) => MetadataHeader.tableStream.ResolveToken(instr.ReadUInt32());


    string ToIlPtrBase(uint value)
    {
        return "IL_" + AddILNameSpace2(value.ToString("x"), 4, "0");
    }

    string AddILNameSpace2(string name, int size, string defaultStr = " ")
    {
        string str = "";
        for (int i = name.Length; i < size; i++)
        {
            str += defaultStr;
        }

        str += name;
        return str;
    }

    void AddILNameSpace(string name, int size, string defaultStr = " ")
    {
        stringBuilder.Append(name);
        for (int i = name.Length; i < size; i++)
        {
            stringBuilder.Append(defaultStr);
        }
    }

    TreeNode AddClrNode(string text, TreeNode parent = null)
    {
        TreeNode node = new TreeNode();
        node.Text = text;
        if (parent != null)
            parent.Nodes.Add(node);
        else
            this.Nodes.Add(node);
        return node;
    }
}