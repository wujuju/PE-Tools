using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
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
            tabControl.Size = new Size(this.Width - 200 - 60, this.Height - 60);
            songsDataGridView.Size = clrTreeView.Size = tabControl.Size - new Size(0, 20);
        }

        private TreeView treeView;
        private TreeNode rootNode;
        private TreeNode classView;
        private PeInfo info;
        private DataGridView songsDataGridView;

        private Dictionary<string, MDTable> tablesMaps = new Dictionary<string, MDTable>();

        // private ScrollableControl scrollableControl;
        private TableStream tableStream;
        private TabControl tabControl;
        private TabPage[] tabPages = new TabPage[2];
        private CLRTreeView clrTreeView;

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

            tabControl = new TabControl();
            tabControl.Location = new System.Drawing.Point(10, 25);
            tabControl.SelectedIndex = 0;
            tabControl.Size = new System.Drawing.Size(this.Width - 40, this.Height - 75);
            tabControl.TabIndex = 0;
            tabControl.Location = new Point(225, 12);
            Controls.Add(tabControl);

            songsDataGridView = new DataGridView();
            songsDataGridView.ReadOnly = true;
            songsDataGridView.DataSourceChanged += MainForm.windowDataGridChange2;
            songsDataGridView.RowHeadersVisible = false;
            songsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            var tabPage = new TabPage();
            tabPage.Controls.Add(songsDataGridView);
            tabPages[0] = tabPage;

            clrTreeView = new CLRTreeView();
            clrTreeView.Init(info);
            tabPage = new TabPage();
            tabPage.Controls.Add(clrTreeView);
            tabPages[1] = tabPage;

            TreeNode tablesNode = new TreeNode();
            tablesNode.Text = "Tables";
            rootNode.Nodes.Add(tablesNode);

            metadataHeader = info.clrDirectory.metadataHeader;
            tableStream = MetadataHeader.tableStream;
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

            AddNode(MetadataHeader.StringHeapStream);
            AddNode(MetadataHeader.UsHeapStream);
            AddNode(MetadataHeader.BlobHeapStream);
            AddNode(MetadataHeader.GuidHeapStream);
            AddNode(MetadataHeader.PdbHeapStream);
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
                typeNode.Text = MetadataHeader.StringHeapStream.Read(typeDef.Name);
                typeNode.Name = "Class_" + i;
                node.Nodes.Add(typeNode);
            }

            classView.Nodes[0].ExpandAll();
            classView.Expand();
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
                    ShowTabPageByIndex(0, selectedNode.Name);
                    ExcuteStream(selectedNode.Name);
                }
                else if (selectedNode.Name.StartsWith("Class_"))
                {
                    var index = Convert.ToUInt32(selectedNode.Name.Substring(6, selectedNode.Name.Length - 6));
                    var typeDef = clrTreeView.ExcuteClass(index, tabControl);
                    Clipboard.SetText(typeDef.name);
                    ShowTabPageByIndex(1, typeDef.name);
                }
                else
                {
                    ShowTabPageByIndex(0, selectedNode.Name);
                    ExcuteTables(selectedNode.Name);
                }
            }
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
                    list = MetadataHeader.StringHeapStream.list;
                    value = "ASCII";
                    break;
                case "#US":
                    list = MetadataHeader.UsHeapStream.list;
                    value = "UNICODE";
                    break;
                case "#Blob":
                    list = MetadataHeader.BlobHeapStream.list;
                    value = "BYTE";
                    break;
                case "#GUID":
                    list = MetadataHeader.GuidHeapStream.list;
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
                        row[index] = PETools.GetString(i + 1, bytesArray.ReadUInt16(), column);
                    }
                    else if (column.size == 4)
                    {
                        row[index] = PETools.GetString(i + 1, bytesArray.ReadUInt32(), column);
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

        void ShowTabPageByIndex(int index, string text)
        {
            for (int i = tabPages.Length - 1; i >= 0; i--)
            {
                tabControl.Controls.Remove(tabPages[i]);
            }

            var control = tabPages[index];
            tabControl.Controls.Add(control);
            control.Text = text;
        }
    }
}