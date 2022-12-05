using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace PE_Tools
{
    public partial class ExportForm : Form
    {
        public ExportForm()
        {
            InitializeComponent();
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(680, 600);
        }

        private DataGridView songsDataGridView;

        public void Init(PeInfo info)
        {
            var data = info._ExportDirectory;
            Type clsType = data.GetType();
            var fields = clsType.GetFields();
            MainForm.CreateTabInfoByFields(data, fields, 250, Controls, 12, 12);

            songsDataGridView = new DataGridView();
            songsDataGridView.ReadOnly = true;
            songsDataGridView.Location = new Point(12, 230);
            songsDataGridView.Size = new Size(this.Width - 40, 300);
            songsDataGridView.DataSourceChanged += MainForm.windowDataGridChange2;
            songsDataGridView.RowHeadersVisible = false;
            songsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            songsDataGridView.AutoSize = false;
            Controls.Add(songsDataGridView);
            DataTable ReturnTable = new DataTable("");
            ReturnTable.Columns.Add("Ordinals");
            ReturnTable.Columns.Add("Size");
            ReturnTable.Columns.Add("Value16");
            ReturnTable.Columns.Add("Value10");
            ReturnTable.Columns.Add("ASCII");

            for (int i = 0; i < data.AddressOfNamesList.Count - 100; i++)
            {
                ReturnTable.Rows.Add(new string[]
                {
                    PETools.GetInt(data.AddressOfNameOrdinalsList[i]).ToString(),
                    PETools.GetHexString(data.AddressOfFunctionsList[i]),
                    PETools.GetHexString(data.AddressOfNamesList[i]),
                    PETools.GetHexString(data.NameList[i], "ASCII")
                });
            }

            songsDataGridView.DataSource = ReturnTable;
        }
    }
}