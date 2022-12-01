using System;
using System.Collections.Generic;
using System.Data;
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
        }

        private TreeView treeView;
        private TreeNode rootNode;
        private PeInfo info;
        private DataGridView songsDataGridView;
        private Dictionary<string, MDTable> tablesMaps = new Dictionary<string, MDTable>();

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
            songsDataGridView.Size = new Size(this.Width - 200 - 60, this.Height - 60);
            songsDataGridView.DataSourceChanged += MainForm.windowDataGridChange2;
            songsDataGridView.RowHeadersVisible = false;
            songsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            songsDataGridView.AutoSize = true;
            var scrollableControl = new ScrollableControl();
            scrollableControl.AutoScroll = true;
            scrollableControl.Size = songsDataGridView.Size + new Size(2, 0);
            scrollableControl.Location = new Point(225, 12);
            scrollableControl.Controls.Add(songsDataGridView);
            Controls.Add(scrollableControl);

            TreeNode tablesNode = new TreeNode();
            tablesNode.Text = "Tables";
            rootNode.Nodes.Add(tablesNode);

            metadataHeader = info.clrDirectory.metadataHeader;
            var tableStream = metadataHeader.tableStream;
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
            if (selectedNode != null)
            {
                if (selectedNode.Parent == rootNode)
                {
                    if (selectedNodeName == selectedNode.Name)
                        return;
                    selectedNodeName = selectedNode.Name;
                    DataTable ReturnTable = new DataTable("");
                    ReturnTable.Columns.Add("Offset");
                    ReturnTable.Columns.Add("Length");
                    ReturnTable.Columns.Add("Value");
                    List<HeapStreamString> list = null;
                    string value = "";
                    switch (selectedNode.Name)
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

                    int offset = 0;
                    foreach (var key in list)
                    {
                        ReturnTable.Rows.Add(new string[]
                        {
                            PETools.Convert16String(key.offset),
                            PETools.Convert16String(key.length),
                            value == "" ? new Guid(key.value).ToString() : PeInfo.GetString(key.value, value)
                        });
                        offset += key.length;
                    }

                    songsDataGridView.DataSource = ReturnTable;
                }
                else
                {
                    if (tablesMaps.TryGetValue(selectedNode.Name, out MDTable table))
                    {
                        var tableStream = metadataHeader.tableStream;
                        DataTable ReturnTable = new DataTable("");
                        ReturnTable.Columns.Add("RID");
                        ReturnTable.Columns.Add("Token");
                        ReturnTable.Columns.Add("Offset");
                        foreach (var column in table.tableInfo.columns)
                        {
                            ReturnTable.Columns.Add(column.name);
                        }

                        var bytesArray = tableStream.reader;
                        var offset = table.tableInfo.offset;
                        bytesArray.Position = tableStream.tablesPos + offset;
                        for (int i = 0; i < table.numRows; i++)
                        {
                            var length = table.tableInfo.columns.Length;
                            string[] row = new string[length + 3];
                            row[0] = (i + 1).ToString();
                            row[1] = PETools.Convert16String((((int)table.table << 4 * 6) + i + 1));
                            row[2] = PETools.Convert16String(bytesArray.Position);
                            offset += table.tableInfo.size;
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
                                    row[index] = GetString(bytesArray.ReadInt16(), column.columnSize);
                                }
                                else if (column.size == 4)
                                {
                                    row[index] = bytesArray.ReadInt32() + "";
                                }
                                else if (column.size == 8)
                                {
                                    row[index] = bytesArray.ReadInt64() + "";
                                }
                                else
                                {
                                    Console.WriteLine("not find:" + column.size);
                                }

                                switch (column.columnSize)
                                {
                                    case ColumnSize.Int16:
                                        break;
                                }
                                // row[j]=
                            }

                            ReturnTable.Rows.Add(row);
                        }

                        songsDataGridView.DataSource = ReturnTable;
                    }
                }
            }
        }


        string GetString(int offset, ColumnSize type)
        {
            switch (type)
            {
                case ColumnSize.Strings:
                    return metadataHeader.StringHeapStream.Read(offset);
                case ColumnSize.UInt16:
                case ColumnSize.GUID:
                    return offset.ToString();
                case ColumnSize.Int16:
                case ColumnSize.Blob:
                    return offset.ToString("X");
            }

            return offset.ToString() + type;
        }
    }
}