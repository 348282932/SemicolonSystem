namespace SemicolonSystem.Show
{
    partial class OrderImportSettingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderImportSettingForm));
            this.lbl_marginHeard = new System.Windows.Forms.Label();
            this.tbx_marginHeard = new System.Windows.Forms.TextBox();
            this.lbl_marginBottom = new System.Windows.Forms.Label();
            this.tbx_marginBottom = new System.Windows.Forms.TextBox();
            this.btn_confirm = new System.Windows.Forms.Button();
            this.btn_cancel = new System.Windows.Forms.Button();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // lbl_marginHeard
            // 
            this.lbl_marginHeard.AutoSize = true;
            this.lbl_marginHeard.Location = new System.Drawing.Point(37, 34);
            this.lbl_marginHeard.Name = "lbl_marginHeard";
            this.lbl_marginHeard.Size = new System.Drawing.Size(89, 12);
            this.lbl_marginHeard.TabIndex = 2;
            this.lbl_marginHeard.Text = "忽略头部行数：";
            // 
            // tbx_marginHeard
            // 
            this.tbx_marginHeard.Location = new System.Drawing.Point(134, 31);
            this.tbx_marginHeard.Name = "tbx_marginHeard";
            this.tbx_marginHeard.Size = new System.Drawing.Size(53, 21);
            this.tbx_marginHeard.TabIndex = 0;
            this.tbx_marginHeard.Text = "0";
            // 
            // lbl_marginBottom
            // 
            this.lbl_marginBottom.AutoSize = true;
            this.lbl_marginBottom.Location = new System.Drawing.Point(37, 72);
            this.lbl_marginBottom.Name = "lbl_marginBottom";
            this.lbl_marginBottom.Size = new System.Drawing.Size(89, 12);
            this.lbl_marginBottom.TabIndex = 3;
            this.lbl_marginBottom.Text = "忽略底部行数：";
            // 
            // tbx_marginBottom
            // 
            this.tbx_marginBottom.Location = new System.Drawing.Point(134, 69);
            this.tbx_marginBottom.Name = "tbx_marginBottom";
            this.tbx_marginBottom.Size = new System.Drawing.Size(53, 21);
            this.tbx_marginBottom.TabIndex = 1;
            this.tbx_marginBottom.Text = "0";
            // 
            // btn_confirm
            // 
            this.btn_confirm.Location = new System.Drawing.Point(39, 105);
            this.btn_confirm.Name = "btn_confirm";
            this.btn_confirm.Size = new System.Drawing.Size(54, 23);
            this.btn_confirm.TabIndex = 4;
            this.btn_confirm.Text = "确认";
            this.btn_confirm.UseVisualStyleBackColor = true;
            this.btn_confirm.Click += new System.EventHandler(this.btn_confirm_Click);
            // 
            // btn_cancel
            // 
            this.btn_cancel.Location = new System.Drawing.Point(133, 105);
            this.btn_cancel.Name = "btn_cancel";
            this.btn_cancel.Size = new System.Drawing.Size(54, 23);
            this.btn_cancel.TabIndex = 5;
            this.btn_cancel.Text = "取消";
            this.btn_cancel.UseVisualStyleBackColor = true;
            this.btn_cancel.Click += new System.EventHandler(this.btn_cancel_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "所有文件|*.*|Excel文件|*.xlsx|Excel文件|*.xls";
            this.openFileDialog.FilterIndex = 0;
            this.openFileDialog.Title = "导入文件路径";
            // 
            // OrderImportSettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(237, 140);
            this.Controls.Add(this.btn_cancel);
            this.Controls.Add(this.btn_confirm);
            this.Controls.Add(this.tbx_marginBottom);
            this.Controls.Add(this.tbx_marginHeard);
            this.Controls.Add(this.lbl_marginBottom);
            this.Controls.Add(this.lbl_marginHeard);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OrderImportSettingForm";
            this.Text = "订单导入配置";
            this.Load += new System.EventHandler(this.OrderImportSettingForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_marginHeard;
        private System.Windows.Forms.TextBox tbx_marginHeard;
        private System.Windows.Forms.Label lbl_marginBottom;
        private System.Windows.Forms.TextBox tbx_marginBottom;
        private System.Windows.Forms.Button btn_confirm;
        private System.Windows.Forms.Button btn_cancel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
    }
}