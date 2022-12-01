﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static PeInfo;
using static PEInfoType;

namespace PE_Tools
{
    public partial class MainForm : Form
    {
        private TabControl tabControl1;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Create a MenuStrip control with a new window.
            MenuStrip ms = new MenuStrip();
            ToolStripMenuItem windowMenu = new ToolStripMenuItem("File");
            ToolStripMenuItem windowNewMenu =
                new ToolStripMenuItem("Open", null, new EventHandler(windowNewMenu_Click));
            windowMenu.DropDownItems.Add(windowNewMenu);
            ms.MdiWindowListItem = windowMenu;
            ms.Items.Add(windowMenu);
            ms.Dock = DockStyle.Top;

            this.tabControl1 = new TabControl();
            tabControl1.SelectedIndexChanged += tabControl1_SelectedIndexChanged;
            this.SuspendLayout();
            //   
            // tabControl1  
            //   
            this.tabControl1.Location = new System.Drawing.Point(10, 25);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(this.Width - 40, this.Height - 75);
            this.tabControl1.TabIndex = 0;

            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(ms);
            this.ResumeLayout(false);

            // string ss = @"";
            // string[] stringSeparators = new string[] { "\r\n" };
            // string[] lines = ss.Split(stringSeparators, StringSplitOptions.None);
            // for (int i = 0; i < lines.Length; i += 3)
            // {
            //     if (i + 2 >= lines.Length)
            //         break;
            //     Console.WriteLine(lines[i] + "=" + lines[i + 1] + ",//" + lines[i + 2]);
            // }

            this.AllowDrop = true;
            this.DragDrop += Form1_DragDrop;
            this.DragEnter += Form1_DragEnter;
            // OpenPE(@"C:\Users\yangyang.he\Desktop\MAA\mkldnn.dll");
            // OpenPE(@"C:\work\reversecore\example\02\15\bin\notepad_upx.exe");
            OpenPE(@"C:\hybridclr_trial\Assets\StreamingAssets\Assembly-CSharp.dll.bytes");
        }

        private void Form1_DragEnter(object sender, DragEventArgs e) //解析信息
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Link;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void Form1_DragDrop(object sender, DragEventArgs e) //解析信息
        {
            string path = ((System.Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString(); //获得路径
            OpenPE(path);
        }

        private Dictionary<string, Action<TabPage>> tabInitDictionary;
        PeInfo info;

        void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == null)
                return;

            Action<TabPage> action;
            if (tabInitDictionary.TryGetValue(tabControl1.SelectedTab.Text, out action))
            {
                action.Invoke(tabControl1.SelectedTab);
                tabInitDictionary.Remove(tabControl1.SelectedTab.Text);
            }
        }

        void OpenPE(string path)
        {
            PeInfo tmpInfo;
            // try
            {
                tmpInfo = new PeInfo(path);
            }
            // catch (Exception e)
            // {
            //     MessageBox.Show("解析错误:" + e.ToString());
            //     return;
            // }

            info = tmpInfo;
            while (tabControl1.Controls.Count > 0)
            {
                tabControl1.Controls.Remove(tabControl1.Controls[0]);
            }

            tabInitDictionary = new Dictionary<string, Action<TabPage>>();
            tabInitDictionary["Dos头信息"] = CreateDosHeaderPanel;
            tabInitDictionary["PE头信息"] = CreatePEHeaderPanel;
            tabInitDictionary["可选头信息"] = CreateOptionalHeaderPanel;
            tabInitDictionary["节表信息"] = CreateSectionDataPanel;
            tabInitDictionary["数据目录表"] = CreateDirDataPanel;

            foreach (var tabName in tabInitDictionary)
            {
                TabPage page = new TabPage();
                page.Text = tabName.Key;
                tabControl1.Controls.Add(page);
            }

            tabControl1.SelectedIndex = -1;
            // tabControl1.SelectedIndex = 0;
            tabControl1.SelectedIndex = 4;
        }

        void CreateDosHeaderPanel(TabPage tabPage1)
        {
            var data = info._DosHeader;
            Type clsType = data.GetType();
            var fields = clsType.GetFields();
            CreateTabInfoByFields(data, fields, tabControl1.Height, tabPage1.Controls);
        }

        void CreatePEHeaderPanel(TabPage tabPage1)
        {
            var data = info._PEHeader;
            Type clsType = data.GetType();
            var fields = clsType.GetFields();
            CreateTabInfoByFields(data, fields, tabControl1.Height, tabPage1.Controls);
        }

        void CreateOptionalHeaderPanel(TabPage tabPage1)
        {
            var data = info._OptionalHeader;
            Type clsType = data.GetType();
            var fields = clsType.GetFields();
            CreateTabInfoByFields(data, fields, tabControl1.Height, tabPage1.Controls);
        }

        void CreateSectionDataPanel(TabPage tabPage1)
        {
            DataGridView songsDataGridView = new DataGridView();
            songsDataGridView.ReadOnly = true;
            songsDataGridView.Size = new Size(this.Width - 40, this.Height - 75);
            songsDataGridView.DataSourceChanged += windowDataGridChange;
            songsDataGridView.RowHeadersVisible = false;
            songsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            tabPage1.Controls.Add(songsDataGridView);
            songsDataGridView.DataSource = info.TableSectionData();
        }

        void CreateDirDataPanel(TabPage tabPage1)
        {
            DataGridView songsDataGridView = new DataGridView();
            songsDataGridView.ReadOnly = true;
            songsDataGridView.Location = new Point(35, 0);
            songsDataGridView.Size = new Size(this.Width - 40, this.Height - 75);
            songsDataGridView.DataSourceChanged += windowDataGridChange2;
            songsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            songsDataGridView.RowHeadersVisible = false;
            string[] btnArray = { "导出", "导入", "CLR" };
            int[] btnPosArray = { 0, 1, 14 };
            for (int i = 0; i < btnArray.Length; i++)
            {
                Button bntinfo = new Button();
                bntinfo.Size = new Size(30, 20);
                bntinfo.Location = new Point(0, 24 + btnPosArray[i] * 23);
                bntinfo.Text = "?";
                bntinfo.Click += OnDirBtnClick;
                bntinfo.Name = btnArray[i];
                tabPage1.Controls.Add(bntinfo);
            }

            tabPage1.Controls.Add(songsDataGridView);
            songsDataGridView.DataSource = info.TableOptionalDirAttrib();
            OnDirBtnClick(tabPage1.Controls.Find("CLR",false)[0], null);
        }

        void OnDirBtnClick(object sender, EventArgs e)
        {
            Button bntinfo = sender as Button;
            if (bntinfo == null)
                return;
            switch (bntinfo.Name)
            {
                case "导出":
                    if (info._ExportDirectory != null)
                    {
                        ExportForm exportForm = new ExportForm();
                        exportForm.Init(info);
                        exportForm.ShowDialog();
                        return;
                    }

                    break;
                case "导入":
                    if (info._ImportDirectory != null)
                    {
                        ImportForm importForm = new ImportForm();
                        importForm.Init(info);
                        importForm.ShowDialog();
                        return;
                    }

                    break;
                case "CLR":
                    if (info.clrDirectory != null)
                    {
                        MetaForm importForm = new MetaForm();
                        importForm.Init(info);
                        importForm.ShowDialog();
                        return;
                    }
                    
                    break;
            }

            MessageBox.Show(string.Format("没有{0}表!", bntinfo.Name));
        }

        public static void CreateTabInfoByFields(object obj, FieldInfo[] fields, int height,
            Control.ControlCollection control, int sx = 4, int sy = 4)
        {
            int startX = sx;
            int startY = sy;
            int rowCount = height / 51;
            int rowHeight = 46;
            int rowWidth = 200;
            for (int i = 0; i < fields.Length; i++)
            {
                var field = fields[i];
                if (i > 0 && i % rowCount == 0)
                    startX += rowWidth + 20;
                var attributes = field.GetCustomAttributes(false);
                if (attributes.Length == 0)
                    continue;
                HeadAttribute dmAttr = (HeadAttribute)attributes[0];
                Label label1 = new Label();
                label1.AutoSize = true;
                label1.Location = new Point(startX, startY + i % rowCount * rowHeight);
                label1.Size = new Size(41, 12);
                label1.TabIndex = 6;
                label1.Text = string.Format("{1}({0})", field.Name, dmAttr.name);
                // label1.Text = dmAttr.name;

                TextBox text = new TextBox();
                text.ReadOnly = true;
                text.Location = new Point(startX, startY + i % rowCount * rowHeight + 16);
                text.Size = new Size(rowWidth, 21);
                if (dmAttr.tpyeName != "")
                {
                    var intValue = PeInfo.GetInt2(field.GetValue(obj) as byte[]);
                    switch (dmAttr.tpyeName)
                    {
                        case "EnMagic":
                            text.Text = Enum.GetName(typeof(EnMagic), intValue);
                            break;
                        case "EnMachine":
                            text.Text = Enum.GetName(typeof(EnMachine), intValue);
                            break;
                        case "EnSubsystem":
                            text.Text = Enum.GetName(typeof(EnSubsystem), intValue);
                            break;
                        case "EnDLLCharacteristic":
                            CreateComboBox(typeof(EnDLLCharacteristic), text, control, label1, intValue);
                            continue;
                        case "EnImageCharacteristic":
                            CreateComboBox(typeof(EnImageCharacteristic), text, control, label1, intValue);
                            continue;
                        case "String":
                            text.Text = PeInfo.GetString(field.GetValue(obj) as byte[], "ASCII");
                            break;
                        case "Time":
                            var ms_date1970 = new DateTime(1970, 1, 1, 0, 0, 0);
                            ms_date1970 = ms_date1970.AddSeconds(intValue);
                            text.Text = ms_date1970.ToString();
                            break;
                    }
                }
                else
                    text.Text = PeInfo.GetString(field.GetValue(obj) as byte[]);

                control.Add(label1);
                control.Add(text);
            }
        }

        public static void CreateComboBox(Type type, TextBox text, Control.ControlCollection controls, Label label1,
            int intValue)
        {
            var combox = new ComboBox();
            foreach (var opParam in Enum.GetValues(type))
            {
                var bbb = intValue & (int)opParam;
                if (bbb > 0)
                    combox.Items.Add("√" + opParam);
            }

            combox.Size = text.Size;
            combox.Location = text.Location;
            combox.SelectedIndex = 0;
            controls.Add(combox);
            controls.Add(label1);
        }

        void windowDataGridChange(object sender, EventArgs e)
        {
            DataGridView songsDataGridView = sender as DataGridView;
            for (int i = 0; i < songsDataGridView.Rows.Count; i++)
            {
                if (i % 10 == 0)
                    songsDataGridView.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
            }

            windowDataGridChange2(sender, e);
        }

        public static void windowDataGridChange2(object sender, EventArgs e)
        {
            DataGridView songsDataGridView = sender as DataGridView;
            for (int i = 0; i < songsDataGridView.Columns.Count; i++)
            {
                songsDataGridView.Columns[i].SortMode = DataGridViewColumnSortMode.NotSortable;
                songsDataGridView.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            }

            for (int i = 0; i < songsDataGridView.Rows.Count; i++)
            {
                songsDataGridView.Rows[i].Resizable = DataGridViewTriState.False;
            }
        }

        void windowNewMenu_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true; //该值确定是否可以选择多个文件
            dialog.Title = "请选择文件夹";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string file = dialog.FileName;
                OpenPE(file);
            }
        }
    }
}