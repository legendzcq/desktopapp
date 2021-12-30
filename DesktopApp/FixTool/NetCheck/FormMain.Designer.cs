namespace NetCheck
{
	partial class FormMain
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.tbMessage = new System.Windows.Forms.TextBox();
			this.panel1 = new System.Windows.Forms.Panel();
			this.BtnSetCM = new System.Windows.Forms.Button();
			this.BtnSetCT = new System.Windows.Forms.Button();
			this.BtnSetNone = new System.Windows.Forms.Button();
			this.BtnSetUC = new System.Windows.Forms.Button();
			this.BtnStart = new System.Windows.Forms.Button();
			this.CbList = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.TbCode = new System.Windows.Forms.TextBox();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// tbMessage
			// 
			this.tbMessage.BackColor = System.Drawing.Color.White;
			this.tbMessage.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tbMessage.Location = new System.Drawing.Point(0, 34);
			this.tbMessage.Multiline = true;
			this.tbMessage.Name = "tbMessage";
			this.tbMessage.ReadOnly = true;
			this.tbMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.tbMessage.Size = new System.Drawing.Size(659, 329);
			this.tbMessage.TabIndex = 0;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.TbCode);
			this.panel1.Controls.Add(this.BtnSetCM);
			this.panel1.Controls.Add(this.BtnSetCT);
			this.panel1.Controls.Add(this.BtnSetNone);
			this.panel1.Controls.Add(this.BtnSetUC);
			this.panel1.Controls.Add(this.BtnStart);
			this.panel1.Controls.Add(this.CbList);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(659, 34);
			this.panel1.TabIndex = 1;
			// 
			// BtnSetCM
			// 
			this.BtnSetCM.Location = new System.Drawing.Point(603, 5);
			this.BtnSetCM.Name = "BtnSetCM";
			this.BtnSetCM.Size = new System.Drawing.Size(44, 23);
			this.BtnSetCM.TabIndex = 3;
			this.BtnSetCM.Text = "移动";
			this.BtnSetCM.UseVisualStyleBackColor = true;
			this.BtnSetCM.Click += new System.EventHandler(this.BtnSetCM_Click);
			// 
			// BtnSetCT
			// 
			this.BtnSetCT.Location = new System.Drawing.Point(553, 5);
			this.BtnSetCT.Name = "BtnSetCT";
			this.BtnSetCT.Size = new System.Drawing.Size(44, 23);
			this.BtnSetCT.TabIndex = 3;
			this.BtnSetCT.Text = "电信";
			this.BtnSetCT.UseVisualStyleBackColor = true;
			this.BtnSetCT.Click += new System.EventHandler(this.BtnSetCT_Click);
			// 
			// BtnSetNone
			// 
			this.BtnSetNone.Location = new System.Drawing.Point(453, 5);
			this.BtnSetNone.Name = "BtnSetNone";
			this.BtnSetNone.Size = new System.Drawing.Size(44, 23);
			this.BtnSetNone.TabIndex = 3;
			this.BtnSetNone.Text = "未设";
			this.BtnSetNone.UseVisualStyleBackColor = true;
			this.BtnSetNone.Click += new System.EventHandler(this.BtnSetNone_Click);
			// 
			// BtnSetUC
			// 
			this.BtnSetUC.Location = new System.Drawing.Point(503, 5);
			this.BtnSetUC.Name = "BtnSetUC";
			this.BtnSetUC.Size = new System.Drawing.Size(44, 23);
			this.BtnSetUC.TabIndex = 3;
			this.BtnSetUC.Text = "联通";
			this.BtnSetUC.UseVisualStyleBackColor = true;
			this.BtnSetUC.Click += new System.EventHandler(this.BtnSetUC_Click);
			// 
			// BtnStart
			// 
			this.BtnStart.Location = new System.Drawing.Point(346, 5);
			this.BtnStart.Name = "BtnStart";
			this.BtnStart.Size = new System.Drawing.Size(75, 23);
			this.BtnStart.TabIndex = 2;
			this.BtnStart.Text = "开始";
			this.BtnStart.UseVisualStyleBackColor = true;
			this.BtnStart.Click += new System.EventHandler(this.BtnStart_Click);
			// 
			// CbList
			// 
			this.CbList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.CbList.FormattingEnabled = true;
			this.CbList.Location = new System.Drawing.Point(213, 7);
			this.CbList.Name = "CbList";
			this.CbList.Size = new System.Drawing.Size(127, 20);
			this.CbList.TabIndex = 1;
			this.CbList.SelectedIndexChanged += new System.EventHandler(this.CbList_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(166, 10);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(41, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "网校：";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(12, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(65, 12);
			this.label2.TabIndex = 0;
			this.label2.Text = "学员代码：";
			// 
			// TbCode
			// 
			this.TbCode.Location = new System.Drawing.Point(83, 6);
			this.TbCode.Name = "TbCode";
			this.TbCode.Size = new System.Drawing.Size(77, 21);
			this.TbCode.TabIndex = 4;
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(659, 363);
			this.Controls.Add(this.tbMessage);
			this.Controls.Add(this.panel1);
			this.Name = "FormMain";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "正保网络检查工具";
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbMessage;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox CbList;
		private System.Windows.Forms.Button BtnStart;
		private System.Windows.Forms.Button BtnSetCM;
		private System.Windows.Forms.Button BtnSetCT;
		private System.Windows.Forms.Button BtnSetUC;
		private System.Windows.Forms.Button BtnSetNone;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox TbCode;

	}
}

