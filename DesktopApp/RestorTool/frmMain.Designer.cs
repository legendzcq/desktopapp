namespace RestorTool
{
    partial class frmMain
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
            this.btnRestor = new System.Windows.Forms.Button();
            this.rchFile = new System.Windows.Forms.RichTextBox();
            this.btnClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnRestor
            // 
            this.btnRestor.Location = new System.Drawing.Point(12, 12);
            this.btnRestor.Name = "btnRestor";
            this.btnRestor.Size = new System.Drawing.Size(65, 23);
            this.btnRestor.TabIndex = 0;
            this.btnRestor.Text = "修复";
            this.btnRestor.UseVisualStyleBackColor = true;
            this.btnRestor.Click += new System.EventHandler(this.btnRestor_Click);
            // 
            // rchFile
            // 
            this.rchFile.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.rchFile.Font = new System.Drawing.Font("SimSun", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.rchFile.Location = new System.Drawing.Point(0, 47);
            this.rchFile.Name = "rchFile";
            this.rchFile.ReadOnly = true;
            this.rchFile.Size = new System.Drawing.Size(464, 323);
            this.rchFile.TabIndex = 1;
            this.rchFile.Text = "";
            // 
            // btnClose
            // 
            this.btnClose.Location = new System.Drawing.Point(83, 12);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(65, 23);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "退出";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 370);
            this.Controls.Add(this.rchFile);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.btnRestor);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmMain";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "修复工具";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnRestor;
        private System.Windows.Forms.RichTextBox rchFile;
        private System.Windows.Forms.Button btnClose;
    }
}

