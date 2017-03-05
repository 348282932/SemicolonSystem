namespace SemicolonSystem.Show
{
    partial class ActivationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ActivationForm));
            this.lbl_Tip = new System.Windows.Forms.Label();
            this.lbl_key = new System.Windows.Forms.Label();
            this.tbx_ActivationCode = new System.Windows.Forms.TextBox();
            this.lbl_ActivationCode = new System.Windows.Forms.Label();
            this.btn_Activation = new System.Windows.Forms.Button();
            this.btn_Copy = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbl_Tip
            // 
            this.lbl_Tip.AutoSize = true;
            this.lbl_Tip.Location = new System.Drawing.Point(86, 95);
            this.lbl_Tip.Name = "lbl_Tip";
            this.lbl_Tip.Size = new System.Drawing.Size(89, 12);
            this.lbl_Tip.TabIndex = 0;
            this.lbl_Tip.Text = "电脑唯一标识：";
            // 
            // lbl_key
            // 
            this.lbl_key.AutoSize = true;
            this.lbl_key.Location = new System.Drawing.Point(175, 95);
            this.lbl_key.Name = "lbl_key";
            this.lbl_key.Size = new System.Drawing.Size(0, 12);
            this.lbl_key.TabIndex = 1;
            // 
            // tbx_ActivationCode
            // 
            this.tbx_ActivationCode.Location = new System.Drawing.Point(177, 41);
            this.tbx_ActivationCode.Name = "tbx_ActivationCode";
            this.tbx_ActivationCode.Size = new System.Drawing.Size(234, 21);
            this.tbx_ActivationCode.TabIndex = 2;
            // 
            // lbl_ActivationCode
            // 
            this.lbl_ActivationCode.AutoSize = true;
            this.lbl_ActivationCode.Location = new System.Drawing.Point(86, 44);
            this.lbl_ActivationCode.Name = "lbl_ActivationCode";
            this.lbl_ActivationCode.Size = new System.Drawing.Size(89, 12);
            this.lbl_ActivationCode.TabIndex = 3;
            this.lbl_ActivationCode.Text = "请输入激活码：";
            // 
            // btn_Activation
            // 
            this.btn_Activation.Location = new System.Drawing.Point(428, 41);
            this.btn_Activation.Name = "btn_Activation";
            this.btn_Activation.Size = new System.Drawing.Size(58, 21);
            this.btn_Activation.TabIndex = 4;
            this.btn_Activation.Text = "激活";
            this.btn_Activation.UseVisualStyleBackColor = true;
            this.btn_Activation.Click += new System.EventHandler(this.btn_Activation_Click);
            // 
            // btn_Copy
            // 
            this.btn_Copy.Location = new System.Drawing.Point(428, 91);
            this.btn_Copy.Name = "btn_Copy";
            this.btn_Copy.Size = new System.Drawing.Size(58, 21);
            this.btn_Copy.TabIndex = 4;
            this.btn_Copy.Text = "复制";
            this.btn_Copy.UseVisualStyleBackColor = true;
            this.btn_Copy.Click += new System.EventHandler(this.btn_Copy_Click);
            // 
            // ActivationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(575, 143);
            this.Controls.Add(this.btn_Copy);
            this.Controls.Add(this.btn_Activation);
            this.Controls.Add(this.lbl_ActivationCode);
            this.Controls.Add(this.tbx_ActivationCode);
            this.Controls.Add(this.lbl_key);
            this.Controls.Add(this.lbl_Tip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ActivationForm";
            this.Text = "请联系厂商获取激活码";
            this.Load += new System.EventHandler(this.ActivationForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_Tip;
        private System.Windows.Forms.Label lbl_key;
        private System.Windows.Forms.TextBox tbx_ActivationCode;
        private System.Windows.Forms.Label lbl_ActivationCode;
        private System.Windows.Forms.Button btn_Activation;
        private System.Windows.Forms.Button btn_Copy;
    }
}