namespace SemicolonSystem.Show
{
    partial class MainForm
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
            this.btn_ImportRule = new System.Windows.Forms.Button();
            this.openRuleFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.btn_ImportOrder = new System.Windows.Forms.Button();
            this.btn_SetWeight = new System.Windows.Forms.Button();
            this.btn_Matching = new System.Windows.Forms.Button();
            this.saveResultFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.lbl_DownRule = new System.Windows.Forms.Label();
            this.lbl_DownOrder = new System.Windows.Forms.Label();
            this.saveTemplateFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.SuspendLayout();
            // 
            // btn_ImportRule
            // 
            this.btn_ImportRule.Location = new System.Drawing.Point(103, 27);
            this.btn_ImportRule.Name = "btn_ImportRule";
            this.btn_ImportRule.Size = new System.Drawing.Size(101, 50);
            this.btn_ImportRule.TabIndex = 0;
            this.btn_ImportRule.Text = "导入规则";
            this.btn_ImportRule.UseVisualStyleBackColor = true;
            this.btn_ImportRule.Click += new System.EventHandler(this.btn_ImportRule_Click);
            // 
            // openRuleFileDialog
            // 
            this.openRuleFileDialog.Filter = "*.xls|*.xlsx";
            this.openRuleFileDialog.FilterIndex = 0;
            this.openRuleFileDialog.Title = "导入文件路径";
            // 
            // btn_ImportOrder
            // 
            this.btn_ImportOrder.Location = new System.Drawing.Point(103, 98);
            this.btn_ImportOrder.Name = "btn_ImportOrder";
            this.btn_ImportOrder.Size = new System.Drawing.Size(101, 50);
            this.btn_ImportOrder.TabIndex = 1;
            this.btn_ImportOrder.Text = "导入订单";
            this.btn_ImportOrder.UseVisualStyleBackColor = true;
            this.btn_ImportOrder.Click += new System.EventHandler(this.btn_ImportOrder_Click);
            // 
            // btn_SetWeight
            // 
            this.btn_SetWeight.Location = new System.Drawing.Point(103, 169);
            this.btn_SetWeight.Name = "btn_SetWeight";
            this.btn_SetWeight.Size = new System.Drawing.Size(101, 50);
            this.btn_SetWeight.TabIndex = 2;
            this.btn_SetWeight.Text = "设置权重";
            this.btn_SetWeight.UseVisualStyleBackColor = true;
            this.btn_SetWeight.Click += new System.EventHandler(this.btn_SetWeight_Click);
            // 
            // btn_Matching
            // 
            this.btn_Matching.Location = new System.Drawing.Point(103, 240);
            this.btn_Matching.Name = "btn_Matching";
            this.btn_Matching.Size = new System.Drawing.Size(101, 50);
            this.btn_Matching.TabIndex = 3;
            this.btn_Matching.Text = "开始匹配";
            this.btn_Matching.UseVisualStyleBackColor = true;
            this.btn_Matching.Click += new System.EventHandler(this.btn_Matching_Click);
            // 
            // saveResultFileDialog
            // 
            this.saveResultFileDialog.FileName = "匹配结果";
            this.saveResultFileDialog.Filter = "*.xls|*.xlsx";
            this.saveResultFileDialog.FilterIndex = 0;
            // 
            // lbl_DownRule
            // 
            this.lbl_DownRule.AutoSize = true;
            this.lbl_DownRule.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_DownRule.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_DownRule.ForeColor = System.Drawing.Color.Blue;
            this.lbl_DownRule.Location = new System.Drawing.Point(13, 311);
            this.lbl_DownRule.Name = "lbl_DownRule";
            this.lbl_DownRule.Size = new System.Drawing.Size(77, 12);
            this.lbl_DownRule.TabIndex = 4;
            this.lbl_DownRule.Text = "获取规则模版";
            this.lbl_DownRule.Click += new System.EventHandler(this.lbl_DownRule_Click);
            // 
            // lbl_DownOrder
            // 
            this.lbl_DownOrder.AutoSize = true;
            this.lbl_DownOrder.BackColor = System.Drawing.SystemColors.Control;
            this.lbl_DownOrder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lbl_DownOrder.ForeColor = System.Drawing.Color.Blue;
            this.lbl_DownOrder.Location = new System.Drawing.Point(214, 311);
            this.lbl_DownOrder.Name = "lbl_DownOrder";
            this.lbl_DownOrder.Size = new System.Drawing.Size(77, 12);
            this.lbl_DownOrder.TabIndex = 4;
            this.lbl_DownOrder.Text = "获取订单模版";
            this.lbl_DownOrder.Click += new System.EventHandler(this.lbl_DownOrder_Click);
            // 
            // saveTemplateFileDialog
            // 
            this.saveTemplateFileDialog.FileName = "模板";
            this.saveTemplateFileDialog.Filter = "*.xls|*.xlsx";
            this.saveTemplateFileDialog.FilterIndex = 0;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(303, 335);
            this.Controls.Add(this.lbl_DownOrder);
            this.Controls.Add(this.lbl_DownRule);
            this.Controls.Add(this.btn_Matching);
            this.Controls.Add(this.btn_SetWeight);
            this.Controls.Add(this.btn_ImportOrder);
            this.Controls.Add(this.btn_ImportRule);
            this.Name = "MainForm";
            this.Text = "智能分号系统";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_ImportRule;
        private System.Windows.Forms.OpenFileDialog openRuleFileDialog;
        private System.Windows.Forms.Button btn_ImportOrder;
        private System.Windows.Forms.Button btn_SetWeight;
        private System.Windows.Forms.Button btn_Matching;
        private System.Windows.Forms.SaveFileDialog saveResultFileDialog;
        private System.Windows.Forms.Label lbl_DownRule;
        private System.Windows.Forms.Label lbl_DownOrder;
        private System.Windows.Forms.SaveFileDialog saveTemplateFileDialog;
    }
}