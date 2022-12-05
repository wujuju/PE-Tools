using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace PE_Tools
{
    public partial class MetaForm : Form
    {
        public MetaForm()
        {
            InitializeComponent();
            this.Resize += Form1_Resize;
        }

        private void Form1_Resize(object sender, System.EventArgs e)
        {
            treeView.Size = new Size(200, this.Height - 60);
            songsDataGridView.Size = new Size(this.Width - 200 - 60, this.Height - 60);
            scrollableControl.Size = songsDataGridView.Size + new Size(2, 0);
        }

        private TreeView treeView;
        private TreeNode rootNode;
        private TreeNode classView;
        private PeInfo info;
        private DataGridView songsDataGridView;
        private Dictionary<string, MDTable> tablesMaps = new Dictionary<string, MDTable>();
        private ScrollableControl scrollableControl;
        private TableStream tableStream;

        public void Init(PeInfo info)
        {
            this.info = info;

            treeView = new TreeView();
            treeView.NodeMouseClick += treeView1_AfterSelect;
            rootNode = new TreeNode();
            rootNode.Text = "Metadata";
            treeView.Size = new Size(200, this.Height - 60);
            treeView.Location = new Point(12, 12);

            rootNode.Expand();
            treeView.Nodes.Add(rootNode);
            Controls.Add(treeView);

            songsDataGridView = new DataGridView();
            songsDataGridView.ReadOnly = true;
            songsDataGridView.DataSourceChanged += MainForm.windowDataGridChange2;
            songsDataGridView.RowHeadersVisible = false;
            songsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            songsDataGridView.AutoSize = true;
            scrollableControl = new ScrollableControl();
            scrollableControl.AutoScroll = true;
            scrollableControl.Location = new Point(225, 12);
            scrollableControl.Controls.Add(songsDataGridView);
            Controls.Add(scrollableControl);

            TreeNode tablesNode = new TreeNode();
            tablesNode.Text = "Tables";
            rootNode.Nodes.Add(tablesNode);

            metadataHeader = info.clrDirectory.metadataHeader;
            tableStream = metadataHeader.tableStream;
            foreach (var table in tableStream.tables)
            {
                if (table.numRows == 0)
                    continue;
                TreeNode node = new TreeNode();
                node.Name = table.tableInfo.name;
                node.Text = string.Format("{2} {0}({1})", table.tableInfo.name, table.numRows,
                    ((int)table.table).ToString("X"));
                tablesNode.Nodes.Add(node);
                tablesMaps[node.Name] = table;
            }

            AddNode(metadataHeader.StringHeapStream);
            AddNode(metadataHeader.UsHeapStream);
            AddNode(metadataHeader.BlobHeapStream);
            AddNode(metadataHeader.GuidHeapStream);
            AddNode(metadataHeader.PdbHeapStream);
            tablesNode.Expand();

            classView = new TreeNode();
            classView.Text = "Class";
            treeView.Nodes.Add(classView);
            Dictionary<string, TreeNode> nameSpaceMaps = new Dictionary<string, TreeNode>();
            for (uint i = 0; i < tableStream.listTypeDefMD.Length; i++)
            {
                var typeDef = tableStream.listTypeDefMD[i];
                var nameSpace = typeDef.nameSpace;
                if (!nameSpaceMaps.TryGetValue(nameSpace, out var node))
                {
                    node = new TreeNode();
                    node.Text = nameSpace;
                    nameSpaceMaps[nameSpace] = node;
                    classView.Nodes.Add(node);
                }

                var typeNode = new TreeNode();
                typeNode.Text = metadataHeader.StringHeapStream.Read(typeDef.Name);
                typeNode.Name = "Class_" + i;
                node.Nodes.Add(typeNode);
            }

            classView.Expand();
            classView.ExpandAll();
            Form1_Resize(null, null);
        }

        void AddNode(HeapStream heapStream)
        {
            if (heapStream != null)
            {
                var tablesNode = new TreeNode();
                tablesNode.Name = heapStream.name;
                tablesNode.Text = string.Format("{0}({1})", heapStream.name, heapStream.list.Count);
                rootNode.Nodes.Add(tablesNode);
            }
        }

        private string selectedNodeName;
        private MetadataHeader metadataHeader;

        private void treeView1_AfterSelect(object sender, TreeNodeMouseClickEventArgs e)
        {
            var selectedNode = e.Node;
            if (selectedNode != null && selectedNode.Name != "")
            {
                if (selectedNode.Parent == rootNode)
                {
                    ExcuteStream(selectedNode.Name);
                }
                else if (selectedNode.Name.StartsWith("Class_"))
                {
                    var index = Convert.ToUInt32(selectedNode.Name.Substring(6, selectedNode.Name.Length - 6));
                    ExcuteClass(index);
                }
                else
                {
                    ExcuteTables(selectedNode.Name);
                }
            }
        }

        unsafe void ExcuteClass(uint index)
        {
            var typeDef = tableStream.listTypeDefMD[index];
            DataTable ReturnTable = new DataTable("");
            ReturnTable.Columns.Add("RID");
            ReturnTable.Columns.Add("Name");
            foreach (var rid in typeDef.methodRidList)
            {
                RawMethodRow row = tableStream.ResolveMethod(rid);
                ReturnTable.Rows.Add(new string[]
                {
                    rid.ToString(),
                    metadataHeader.StringHeapStream.Read(row.Name)
                });
                var reader = info.bytes;
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
                byte* codeEnd = ip + codeSize;
                while (ip < codeEnd)
                {
                    OpCodeInfo oc = OpCodeInfo.DecodeOpCodeInfo(ip, codeEnd);
                    int opCodeSize = OpCodeInfo.GetOpCodeSize(ip, oc);
                    byte* nextIp = ip + opCodeSize;
                    string param = "   ";
                    var token = *(uint*)(ip + 1);
                    if (oc.name.StartsWith("ldstr"))
                    {
                        param += PETools.GetHexString(
                            metadataHeader.UsHeapStream.ReadUTF16String(token & 0x00FFFFFF),
                            "UNICODE");
                    }


                    switch (oc.id)
                    {
                        case OpcodeEnum.CALLVIRT:
                        case OpcodeEnum.CALL:
                            switch (MDToken.ToTable(token))
                            {
                                case Table.MemberRef:
                                    param += GetString(index, MDToken.ToRID(token), null, ColumnSize.MemberRef);
                                    break;
                            }

                            break;
                        case OpcodeEnum.LDFLD:
                        case OpcodeEnum.NEWARR:
                            param += tableStream.ResolveToken(token);
                            break;
                    }

                    ReturnTable.Rows.Add(new string[]
                    {
                        "",
                        oc.name + "" + param
                    });
                    ip = nextIp;
                }
            }

            songsDataGridView.DataSource = ReturnTable;
        }


        void ExcuteStream(string name)
        {
            if (selectedNodeName == name)
                return;
            selectedNodeName = name;
            DataTable ReturnTable = new DataTable("");
            ReturnTable.Columns.Add("Offset");
            ReturnTable.Columns.Add("Length");
            ReturnTable.Columns.Add("Value");
            List<HeapStreamString> list = null;
            string value = "";
            switch (name)
            {
                case "#Strings":
                    list = metadataHeader.StringHeapStream.list;
                    value = "ASCII";
                    break;
                case "#US":
                    list = metadataHeader.UsHeapStream.list;
                    value = "UNICODE";
                    break;
                case "#Blob":
                    list = metadataHeader.BlobHeapStream.list;
                    value = "BYTE";
                    break;
                case "#GUID":
                    list = metadataHeader.GuidHeapStream.list;
                    break;
            }

            foreach (var key in list)
            {
                ReturnTable.Rows.Add(new string[]
                {
                    PETools.Convert16String(key.offset),
                    PETools.Convert16String(key.length),
                    value == "" ? new Guid(key.value).ToString() : PETools.GetHexString(key.value, value)
                });
            }

            songsDataGridView.DataSource = ReturnTable;
        }

        void ExcuteTables(string selectedNodeName)
        {
            if (!tablesMaps.TryGetValue(selectedNodeName, out MDTable table))
                return;
            DataTable ReturnTable = new DataTable("");
            ReturnTable.Columns.Add("RID");
            ReturnTable.Columns.Add("Token");
            ReturnTable.Columns.Add("Offset");
            foreach (var column in table.tableInfo.columns)
            {
                ReturnTable.Columns.Add(column.name);
            }

            var bytesArray = tableStream.reader;
            bytesArray.Position = table.tableInfo.offset;
            for (uint i = 0; i < table.numRows; i++)
            {
                var length = table.tableInfo.columns.Length;
                string[] row = new string[length + 3];
                row[0] = (i + 1).ToString();
                row[1] = PETools.Convert16String((((uint)table.table << 4 * 6) + i + 1));
                row[2] = PETools.Convert16String(bytesArray.Position);
                for (int j = 0; j < length; j++)
                {
                    var column = table.tableInfo.columns[j];
                    var index = j + 3;

                    if (column.size == 1)
                    {
                        row[index] = bytesArray.ReadInt8() + "";
                    }
                    else if (column.size == 2)
                    {
                        row[index] = GetString(i + 1, bytesArray.ReadUInt16(), column);
                    }
                    else if (column.size == 4)
                    {
                        row[index] = GetString(i + 1, bytesArray.ReadUInt32(), column);
                    }
                    else if (column.size == 8)
                    {
                        row[index] = bytesArray.ReadInt64() + "";
                    }
                    else
                    {
                        Console.WriteLine("not find:" + column.size);
                    }
                }

                ReturnTable.Rows.Add(row);
            }

            songsDataGridView.DataSource = ReturnTable;
        }


        string GetString(uint rid, uint offset, ColumnInfo column, ColumnSize type = ColumnSize.None)
        {
            if (column != null)
                type = column.columnSize;
            string str = "";
            switch (type)
            {
                case ColumnSize.TypeFlags:
                    return ((TypeAttributes)offset).ToString();
                case ColumnSize.FieldFlags:
                    return ((FieldAttributes)offset).ToString();
                case ColumnSize.MethodFlags:
                    return ((MethodAttributes)offset).ToString();
                case ColumnSize.Strings:
                    return metadataHeader.StringHeapStream.Read(offset);
                case ColumnSize.UInt16:
                case ColumnSize.UInt32:
                case ColumnSize.GUID:
                    return offset.ToString();
                case ColumnSize.Int16:
                case ColumnSize.Blob:
                    return offset.ToString("X");
                case ColumnSize.TypeDefOrRef:
                    var row = tableStream.ResolveTypeDefOrRef(offset);
                    if (row is RawTypeDefRow)
                    {
                        var typeDef = row as RawTypeDefRow;
                        return typeDef.name;
                    }

                    if (row is RawTypeRefRow)
                    {
                        var typeRef = row as RawTypeRefRow;
                        return typeRef.name;
                    }

                    return "";
                case ColumnSize.ResolutionScope:
                    return tableStream.ResolveResolutionScope(offset);
                case ColumnSize.MemberRefParent:
                    return tableStream.ResolveMemberRefParent(offset);
                case ColumnSize.MemberRef:
                    var methodRef = tableStream.ResolveMemberRef(offset);
                    if (methodRef != null)
                        return tableStream.ResolveMemberRefParent(methodRef.Class) + "::" + methodRef.name;
                    break;
                case ColumnSize.FieldList:
                    var fieldRidList = tableStream.ResolveTypeDef(rid).fieldRidList;
                    if (fieldRidList.Count == 0)
                        return "";
                    for (int i = 0; i < fieldRidList.Count; i++)
                    {
                        str += tableStream.ResolveField(fieldRidList[i]).name +
                               (i < fieldRidList.Count - 1 ? " | " : "");
                    }

                    return str;
                case ColumnSize.MethodList:
                    var methodRidList = tableStream.ResolveTypeDef(rid).methodRidList;
                    if (methodRidList.Count == 0)
                        return "";
                    for (int i = 0; i < methodRidList.Count; i++)
                    {
                        str += tableStream.ResolveMethod(methodRidList[i]).name +
                               (i < methodRidList.Count - 1 ? " | " : "");
                    }

                    return str;
                case ColumnSize.Method:
                    var method = tableStream.ResolveMethod(offset);
                    if (method != null)
                        return method.name;
                    break;
            }

            return offset.ToString();
        }
    }
}