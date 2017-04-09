namespace SemicolonSystem.Show
{
    partial class MatchingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MatchingForm));
            this.lbl_PositionTitle = new System.Windows.Forms.Label();
            this.lbl_OffsetTitle = new System.Windows.Forms.Label();
            this.lbl_PriorityLevel = new System.Windows.Forms.Label();
            this.btn_confirm = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lbl_PositionTitle
            // 
            this.lbl_PositionTitle.AutoSize = true;
            this.lbl_PositionTitle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_PositionTitle.Location = new System.Drawing.Point(39, 21);
            this.lbl_PositionTitle.Name = "lbl_PositionTitle";
            this.lbl_PositionTitle.Size = new System.Drawing.Size(31, 12);
            this.lbl_PositionTitle.TabIndex = 0;
            this.lbl_PositionTitle.Text = "部位";
            // 
            // lbl_OffsetTitle
            // 
            this.lbl_OffsetTitle.AutoSize = true;
            this.lbl_OffsetTitle.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_OffsetTitle.Location = new System.Drawing.Point(155, 21);
            this.lbl_OffsetTitle.Name = "lbl_OffsetTitle";
            this.lbl_OffsetTitle.Size = new System.Drawing.Size(57, 12);
            this.lbl_OffsetTitle.TabIndex = 0;
            this.lbl_OffsetTitle.Text = "归号范围";
            // 
            // lbl_PriorityLevel
            // 
            this.lbl_PriorityLevel.AutoSize = true;
            this.lbl_PriorityLevel.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.lbl_PriorityLevel.Location = new System.Drawing.Point(285, 21);
            this.lbl_PriorityLevel.Name = "lbl_PriorityLevel";
            this.lbl_PriorityLevel.Size = new System.Drawing.Size(44, 12);
            this.lbl_PriorityLevel.TabIndex = 0;
            this.lbl_PriorityLevel.Text = "优先级";
            // 
            // btn_confirm
            // 
            this.btn_confirm.Location = new System.Drawing.Point(254, 61);
            this.btn_confirm.Name = "btn_confirm";
            this.btn_confirm.Size = new System.Drawing.Size(75, 23);
            this.btn_confirm.TabIndex = 1;
            this.btn_confirm.Text = "确认";
            this.btn_confirm.UseVisualStyleBackColor = true;
            this.btn_confirm.Click += new System.EventHandler(this.btn_confirm_Click);
            // 
            // MatchingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(383, 96);
            this.Controls.Add(this.btn_confirm);
            this.Controls.Add(this.lbl_PriorityLevel);
            this.Controls.Add(this.lbl_OffsetTitle);
            this.Controls.Add(this.lbl_PositionTitle);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MatchingForm";
            this.Text = "设置权重匹配订单";
            this.Load += new System.EventHandler(this.MatchingForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_PositionTitle;
        private System.Windows.Forms.Label lbl_OffsetTitle;
        private System.Windows.Forms.Label lbl_PriorityLevel;
        private System.Windows.Forms.Button btn_confirm;
    }
}