using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using PE_Tools;

public class ImportForm : System.Windows.Forms.Form
{
    #region " Windows Form Designer generated code "

    public ImportForm() : base()
    {
        //This call is required by the Windows Form Designer.
        InitializeComponent();

        //Add any initialization after the InitializeComponent() call
    }

    //Form overrides dispose to clean up the component list.
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (components != null)
            {
                components.Dispose();
            }
        }

        base.Dispose(disposing);
    }

    public ListBox listBox1;
    private Label label1;
    private FlowLayoutPanel flowLayoutPanel1;
    private Label label2;
    private Label label3;
    private Label label4;
    private FlowLayoutPanel flowLayoutPanel2;
    private TextBox textBox1;
    private TextBox textBox2;
    private TextBox textBox3;
    private TextBox textBox4;
    private TextBox textBox5;
    private FlowLayoutPanel flowLayoutPanel3;
    private Label label5;
    private Label label6;
    private Label label7;
    private Label label8;
    private Label label9;
    private Label label10;


    //Required by the Windows Form Designer
    private System.ComponentModel.IContainer components;

    //NOTE: The following procedure is required by the Windows Form Designer
    //It can be modified using the Windows Form Designer.  
    //Do not modify it using the code editor.
    [System.Diagnostics.DebuggerNonUserCode]
    private void InitializeComponent()
    {
        this.listBox1 = new System.Windows.Forms.ListBox();
        this.label1 = new System.Windows.Forms.Label();
        this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
        this.label2 = new System.Windows.Forms.Label();
        this.label3 = new System.Windows.Forms.Label();
        this.label4 = new System.Windows.Forms.Label();
        this.label9 = new System.Windows.Forms.Label();
        this.flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
        this.textBox1 = new System.Windows.Forms.TextBox();
        this.textBox2 = new System.Windows.Forms.TextBox();
        this.textBox3 = new System.Windows.Forms.TextBox();
        this.textBox4 = new System.Windows.Forms.TextBox();
        this.textBox5 = new System.Windows.Forms.TextBox();
        this.flowLayoutPanel3 = new System.Windows.Forms.FlowLayoutPanel();
        this.label5 = new System.Windows.Forms.Label();
        this.label6 = new System.Windows.Forms.Label();
        this.label7 = new System.Windows.Forms.Label();
        this.label8 = new System.Windows.Forms.Label();
        this.label10 = new System.Windows.Forms.Label();
        this.flowLayoutPanel1.SuspendLayout();
        this.flowLayoutPanel2.SuspendLayout();
        this.flowLayoutPanel3.SuspendLayout();
        this.SuspendLayout();
        // 
        // listBox1
        // 
        this.listBox1.FormattingEnabled = true;
        this.listBox1.ItemHeight = 12;
        this.listBox1.Location = new System.Drawing.Point(12, 12);
        this.listBox1.Name = "listBox1";
        this.listBox1.Size = new System.Drawing.Size(145, 292);
        this.listBox1.TabIndex = 0;
        // 
        // label1
        // 
        this.label1.AutoSize = true;
        this.label1.Location = new System.Drawing.Point(8, 5);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(113, 12);
        this.label1.TabIndex = 1;
        this.label1.Text = "OriginalFirstThunk";
        // 
        // flowLayoutPanel1
        // 
        this.flowLayoutPanel1.AutoSize = true;
        this.flowLayoutPanel1.Controls.Add(this.label1);
        this.flowLayoutPanel1.Controls.Add(this.label2);
        this.flowLayoutPanel1.Controls.Add(this.label3);
        this.flowLayoutPanel1.Controls.Add(this.label4);
        this.flowLayoutPanel1.Controls.Add(this.label9);
        this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        this.flowLayoutPanel1.Location = new System.Drawing.Point(179, 12);
        this.flowLayoutPanel1.Name = "flowLayoutPanel1";
        this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(5);
        this.flowLayoutPanel1.Size = new System.Drawing.Size(145, 292);
        this.flowLayoutPanel1.TabIndex = 2;
        // 
        // label2
        // 
        this.label2.AutoSize = true;
        this.label2.Location = new System.Drawing.Point(8, 17);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(83, 12);
        this.label2.TabIndex = 2;
        this.label2.Text = "TimeDateStamp";
        // 
        // label3
        // 
        this.label3.AutoSize = true;
        this.label3.Location = new System.Drawing.Point(8, 29);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(89, 12);
        this.label3.TabIndex = 3;
        this.label3.Text = "ForwarderChain";
        // 
        // label4
        // 
        this.label4.AutoSize = true;
        this.label4.Location = new System.Drawing.Point(8, 41);
        this.label4.Name = "label4";
        this.label4.Size = new System.Drawing.Size(29, 12);
        this.label4.TabIndex = 4;
        this.label4.Text = "Name";
        // 
        // label9
        // 
        this.label9.AutoSize = true;
        this.label9.Location = new System.Drawing.Point(8, 53);
        this.label9.Name = "label9";
        this.label9.Size = new System.Drawing.Size(65, 12);
        this.label9.TabIndex = 5;
        this.label9.Text = "FirstThunk";
        // 
        // flowLayoutPanel2
        // 
        this.flowLayoutPanel2.Controls.Add(this.textBox1);
        this.flowLayoutPanel2.Controls.Add(this.textBox2);
        this.flowLayoutPanel2.Controls.Add(this.textBox3);
        this.flowLayoutPanel2.Controls.Add(this.textBox4);
        this.flowLayoutPanel2.Controls.Add(this.textBox5);
        this.flowLayoutPanel2.Location = new System.Drawing.Point(335, 12);
        this.flowLayoutPanel2.Name = "flowLayoutPanel2";
        this.flowLayoutPanel2.Size = new System.Drawing.Size(164, 292);
        this.flowLayoutPanel2.TabIndex = 3;
        // 
        // textBox1
        // 
        this.textBox1.Location = new System.Drawing.Point(3, 3);
        this.textBox1.Name = "textBox1";
        this.textBox1.Size = new System.Drawing.Size(150, 21);
        this.textBox1.TabIndex = 0;
        // 
        // textBox2
        // 
        this.textBox2.Location = new System.Drawing.Point(3, 30);
        this.textBox2.Name = "textBox2";
        this.textBox2.Size = this.textBox1.Size;
        this.textBox2.TabIndex = 1;
        // 
        // textBox3
        // 
        this.textBox3.Location = new System.Drawing.Point(3, 57);
        this.textBox3.Name = "textBox3";
        this.textBox3.Size = this.textBox1.Size;
        this.textBox3.TabIndex = 2;
        // 
        // textBox4
        // 
        this.textBox4.Location = new System.Drawing.Point(3, 84);
        this.textBox4.Name = "textBox4";
        this.textBox4.Size = this.textBox1.Size;
        this.textBox4.TabIndex = 3;
        // 
        // textBox5
        // 
        this.textBox5.Location = new System.Drawing.Point(3, 111);
        this.textBox5.Name = "textBox5";
        this.textBox5.Size = this.textBox1.Size;
        this.textBox5.TabIndex = 4;
        // 
        // flowLayoutPanel3
        // 
        this.flowLayoutPanel3.AutoSize = true;
        this.flowLayoutPanel3.Controls.Add(this.label5);
        this.flowLayoutPanel3.Controls.Add(this.label6);
        this.flowLayoutPanel3.Controls.Add(this.label7);
        this.flowLayoutPanel3.Controls.Add(this.label8);
        this.flowLayoutPanel3.Controls.Add(this.label10);
        this.flowLayoutPanel3.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        this.flowLayoutPanel3.Location = new System.Drawing.Point(505, 12);
        this.flowLayoutPanel3.Name = "flowLayoutPanel3";
        this.flowLayoutPanel3.Padding = new System.Windows.Forms.Padding(5);
        this.flowLayoutPanel3.Size = new System.Drawing.Size(163, 292);
        this.flowLayoutPanel3.TabIndex = 5;
        // 
        // label5
        // 
        this.label5.AutoSize = true;
        this.label5.Location = new System.Drawing.Point(8, 5);
        this.label5.Name = "label5";
        this.label5.Size = new System.Drawing.Size(113, 12);
        this.label5.TabIndex = 1;
        this.label5.Text = "OriginalFirstThunk";
        // 
        // label6
        // 
        this.label6.AutoSize = true;
        this.label6.Location = new System.Drawing.Point(8, 17);
        this.label6.Name = "label6";
        this.label6.Size = new System.Drawing.Size(83, 12);
        this.label6.TabIndex = 2;
        this.label6.Text = "TimeDateStamp";
        // 
        // label7
        // 
        this.label7.AutoSize = true;
        this.label7.Location = new System.Drawing.Point(8, 29);
        this.label7.Name = "label7";
        this.label7.Size = new System.Drawing.Size(89, 12);
        this.label7.TabIndex = 3;
        this.label7.Text = "ForwarderChain";
        // 
        // label8
        // 
        this.label8.AutoSize = true;
        this.label8.Location = new System.Drawing.Point(8, 41);
        this.label8.Name = "label8";
        this.label8.Size = new System.Drawing.Size(65, 12);
        this.label8.TabIndex = 4;
        this.label8.Text = "FirstThunk";
        // 
        // label10
        // 
        this.label10.AutoSize = true;
        this.label10.Location = new System.Drawing.Point(8, 53);
        this.label10.Name = "label10";
        this.label10.Size = new System.Drawing.Size(65, 12);
        this.label10.TabIndex = 5;
        this.label10.Text = "FirstThunk";
        // 
        // ImportForm
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(680, 600);
        this.Controls.Add(this.flowLayoutPanel2);
        this.Controls.Add(this.flowLayoutPanel1);
        this.Controls.Add(this.listBox1);
        this.Controls.Add(this.flowLayoutPanel3);
        this.Name = "ImportForm";
        this.Load += new System.EventHandler(this.ImportForm_Load);
        this.flowLayoutPanel1.ResumeLayout(false);
        this.flowLayoutPanel1.PerformLayout();
        this.flowLayoutPanel2.ResumeLayout(false);
        this.flowLayoutPanel2.PerformLayout();
        this.flowLayoutPanel3.ResumeLayout(false);
        this.flowLayoutPanel3.PerformLayout();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private Dictionary<string, List<ImportDirectory.ImportDate.FunctionList>> importFunctionList;
    private DataGridView songsDataGridView;
    private PeInfo info;

    public void Init(PeInfo info)
    {
        this.info = info;
        songsDataGridView = new DataGridView();
        songsDataGridView.ReadOnly = true;
        // songsDataGridView.Location = new Point(12, 320);
        songsDataGridView.Size = new Size(this.Width - 40, 260);
        songsDataGridView.DataSourceChanged += MainForm.windowDataGridChange2;
        songsDataGridView.RowHeadersVisible = false;
        songsDataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        songsDataGridView.AutoSize = true;
        var scrollableControl = new ScrollableControl();
        scrollableControl.AutoScroll = true;
        scrollableControl.Size = songsDataGridView.Size + new Size(2, 0);
        scrollableControl.Location = new Point(12, 320);
        scrollableControl.Controls.Add(songsDataGridView);
        Controls.Add(scrollableControl);

        for (int i = 1; i < flowLayoutPanel1.Controls.Count; i++)
        {
            flowLayoutPanel1.Controls[i].Margin = new Padding(0, 30, 0, 0);
            flowLayoutPanel2.Controls[i].Margin = new Padding(0, 20, 0, 0);
            flowLayoutPanel3.Controls[i].Margin = new Padding(0, 30, 0, 0);
        }


        importFunctionList = info.TableImportFunction();
        foreach (var importDate in info._ImportDirectory.ImportList)
        {
            listBox1.Items.Add(PeInfo.GetString(importDate.DLLName, "ASCII"));
        }

        listBox1.SelectedIndexChanged += OnListSelectedIndexChanged;
        listBox1.SelectedIndex = 0;
    }

    void OnListSelectedIndexChanged(object sender, EventArgs e)
    {
        var list2 = flowLayoutPanel2.Controls.GetEnumerator();
        var list3 = flowLayoutPanel3.Controls.GetEnumerator();
        var index = listBox1.SelectedIndex;
        var importData = info._ImportDirectory.ImportList[index];

        Type clsType = importData.GetType();
        var fields = clsType.GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            var field = fields[i];
            var attributes = field.GetCustomAttributes(false);
            if (attributes.Length == 0)
                continue;
            list2.MoveNext();
            list3.MoveNext();
            var text = list2.Current as TextBox;
            HeadAttribute dmAttr = (HeadAttribute)attributes[0];
            text.Text = PeInfo.GetString(field.GetValue(importData) as byte[]);
            var label = list3.Current as Label;
            label.Text = dmAttr.name;
        }

        string item = listBox1.SelectedItem as string;
        List<ImportDirectory.ImportDate.FunctionList> functionLists;
        if (importFunctionList.TryGetValue(item, out functionLists))
        {
            DataTable ReturnTable = new DataTable();
            ReturnTable.Columns.Add("Hint");
            ReturnTable.Columns.Add("Function");

            for (int j = 0; j < functionLists.Count; j++)
            {
                var fun = functionLists[j];
                ReturnTable.Rows.Add(new string[]
                {
                    PeInfo.GetString(fun.FunctionHead),
                    PeInfo.GetString(fun.FunctionName, "ASCII"),
                });
            }

            songsDataGridView.DataSource = ReturnTable;
        }
    }

    #endregion

    private void ImportForm_Load(object sender, EventArgs e)
    {
    }
}