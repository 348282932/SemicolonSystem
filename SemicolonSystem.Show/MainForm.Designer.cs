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
            this.SuspendLayout();
            // 
            // btn_ImportRule
            // 
            this.btn_ImportRule.Location = new System.Drawing.Point(119, 101);
            this.btn_ImportRule.Name = "btn_ImportRule";
            this.btn_ImportRule.Size = new System.Drawing.Size(75, 23);
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
            this.btn_ImportOrder.Location = new System.Drawing.Point(119, 186);
            this.btn_ImportOrder.Name = "btn_ImportOrder";
            this.btn_ImportOrder.Size = new System.Drawing.Size(75, 23);
            this.btn_ImportOrder.TabIndex = 1;
            this.btn_ImportOrder.Text = "导入订单";
            this.btn_ImportOrder.UseVisualStyleBackColor = true;
            this.btn_ImportOrder.Click += new System.EventHandler(this.btn_ImportOrder_Click);
            // 
            // btn_SetWeight
            // 
            this.btn_SetWeight.Location = new System.Drawing.Point(447, 100);
            this.btn_SetWeight.Name = "btn_SetWeight";
            this.btn_SetWeight.Size = new System.Drawing.Size(75, 23);
            this.btn_SetWeight.TabIndex = 2;
            this.btn_SetWeight.Text = "设置权重";
            this.btn_SetWeight.UseVisualStyleBackColor = true;
            this.btn_SetWeight.Click += new System.EventHandler(this.btn_SetWeight_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(909, 426);
            this.Controls.Add(this.btn_SetWeight);
            this.Controls.Add(this.btn_ImportOrder);
            this.Controls.Add(this.btn_ImportRule);
            this.Name = "MainForm";
            this.Text = "智能分号系统";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_ImportRule;
        private System.Windows.Forms.OpenFileDialog openRuleFileDialog;
        private System.Windows.Forms.Button btn_ImportOrder;
        private System.Windows.Forms.Button btn_SetWeight;
    }
}