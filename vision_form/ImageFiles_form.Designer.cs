namespace vision_form
{
    partial class ImageFiles_form
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.but = new System.Windows.Forms.Button();
            this.txtDir = new System.Windows.Forms.TextBox();
            this.labDir = new System.Windows.Forms.Label();
            this.button_exit = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(467, 265);
            this.tabControl1.TabIndex = 33;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.but);
            this.tabPage1.Controls.Add(this.txtDir);
            this.tabPage1.Controls.Add(this.labDir);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(459, 239);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "设置";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // but
            // 
            this.but.Location = new System.Drawing.Point(403, 16);
            this.but.Name = "but";
            this.but.Size = new System.Drawing.Size(39, 23);
            this.but.TabIndex = 2;
            this.but.Text = "...";
            this.but.UseVisualStyleBackColor = true;
            this.but.Click += new System.EventHandler(this.but_Click);
            // 
            // txtDir
            // 
            this.txtDir.Location = new System.Drawing.Point(83, 17);
            this.txtDir.Name = "txtDir";
            this.txtDir.Size = new System.Drawing.Size(313, 21);
            this.txtDir.TabIndex = 1;
            // 
            // labDir
            // 
            this.labDir.AutoSize = true;
            this.labDir.Location = new System.Drawing.Point(13, 21);
            this.labDir.Name = "labDir";
            this.labDir.Size = new System.Drawing.Size(65, 12);
            this.labDir.TabIndex = 0;
            this.labDir.Text = "选择路径：";
            // 
            // button_exit
            // 
            this.button_exit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_exit.Location = new System.Drawing.Point(386, 287);
            this.button_exit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_exit.Name = "button_exit";
            this.button_exit.Size = new System.Drawing.Size(93, 31);
            this.button_exit.TabIndex = 31;
            this.button_exit.Text = "退出";
            this.button_exit.UseVisualStyleBackColor = true;
            this.button_exit.Click += new System.EventHandler(this.button_exit_Click);
            // 
            // button_save
            // 
            this.button_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_save.Location = new System.Drawing.Point(287, 287);
            this.button_save.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(93, 31);
            this.button_save.TabIndex = 32;
            this.button_save.Text = "确认";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // ListFiles_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(491, 331);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button_exit);
            this.Controls.Add(this.button_save);
            this.Name = "ImageFiles_form";
            this.Text = "ImageFiles_form";
            this.Load += new System.EventHandler(this.ImageFiles_form_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button but;
        private System.Windows.Forms.TextBox txtDir;
        private System.Windows.Forms.Label labDir;
        private System.Windows.Forms.Button button_exit;
        private System.Windows.Forms.Button button_save;
    }
}