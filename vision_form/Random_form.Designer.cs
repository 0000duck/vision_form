namespace vision_form
{
    partial class Random_form
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
            this.button_exit = new System.Windows.Forms.Button();
            this.button_save = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.labMin = new System.Windows.Forms.Label();
            this.txtMin = new System.Windows.Forms.TextBox();
            this.labMax = new System.Windows.Forms.Label();
            this.txtMax = new System.Windows.Forms.TextBox();
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // button_exit
            // 
            this.button_exit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_exit.Location = new System.Drawing.Point(305, 296);
            this.button_exit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_exit.Name = "button_exit";
            this.button_exit.Size = new System.Drawing.Size(93, 31);
            this.button_exit.TabIndex = 28;
            this.button_exit.Text = "退出";
            this.button_exit.UseVisualStyleBackColor = true;
            this.button_exit.Click += new System.EventHandler(this.button_exit_Click);
            // 
            // button_save
            // 
            this.button_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_save.Location = new System.Drawing.Point(206, 296);
            this.button_save.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(93, 31);
            this.button_save.TabIndex = 29;
            this.button_save.Text = "确认";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.txtMax);
            this.tabPage1.Controls.Add(this.labMax);
            this.tabPage1.Controls.Add(this.txtMin);
            this.tabPage1.Controls.Add(this.labMin);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(376, 239);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "设置";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(384, 265);
            this.tabControl1.TabIndex = 30;
            // 
            // labMin
            // 
            this.labMin.AutoSize = true;
            this.labMin.Location = new System.Drawing.Point(23, 21);
            this.labMin.Name = "labMin";
            this.labMin.Size = new System.Drawing.Size(53, 12);
            this.labMin.TabIndex = 0;
            this.labMin.Text = "最小值：";
            // 
            // txtMin
            // 
            this.txtMin.Location = new System.Drawing.Point(83, 17);
            this.txtMin.Name = "txtMin";
            this.txtMin.Size = new System.Drawing.Size(146, 21);
            this.txtMin.TabIndex = 1;
            // 
            // labMax
            // 
            this.labMax.AutoSize = true;
            this.labMax.Location = new System.Drawing.Point(23, 59);
            this.labMax.Name = "labMax";
            this.labMax.Size = new System.Drawing.Size(53, 12);
            this.labMax.TabIndex = 0;
            this.labMax.Text = "最大值：";
            // 
            // txtMax
            // 
            this.txtMax.Location = new System.Drawing.Point(83, 55);
            this.txtMax.Name = "txtMax";
            this.txtMax.Size = new System.Drawing.Size(146, 21);
            this.txtMax.TabIndex = 1;
            // 
            // Random_form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(410, 340);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.button_exit);
            this.Controls.Add(this.button_save);
            this.Name = "Random_form";
            this.Text = "Random_form";
            this.Load += new System.EventHandler(this.Random_form_Load);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button_exit;
        private System.Windows.Forms.Button button_save;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox txtMax;
        private System.Windows.Forms.Label labMax;
        private System.Windows.Forms.TextBox txtMin;
        private System.Windows.Forms.Label labMin;
        private System.Windows.Forms.TabControl tabControl1;
    }
}