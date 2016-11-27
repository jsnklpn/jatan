namespace TestUI.Controls
{
    partial class HexPointControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label2 = new System.Windows.Forms.Label();
            this.hexagonControl2 = new TestUI.Controls.HexagonControl();
            this.label1 = new System.Windows.Forms.Label();
            this.hexagonControl1 = new TestUI.Controls.HexagonControl();
            this.label3 = new System.Windows.Forms.Label();
            this.hexagonControl3 = new TestUI.Controls.HexagonControl();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 40);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Hexagon 2:";
            // 
            // hexagonControl2
            // 
            this.hexagonControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexagonControl2.Location = new System.Drawing.Point(72, 33);
            this.hexagonControl2.Name = "hexagonControl2";
            this.hexagonControl2.Size = new System.Drawing.Size(320, 30);
            this.hexagonControl2.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Hexagon 1:";
            // 
            // hexagonControl1
            // 
            this.hexagonControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexagonControl1.Location = new System.Drawing.Point(72, 3);
            this.hexagonControl1.Name = "hexagonControl1";
            this.hexagonControl1.Size = new System.Drawing.Size(320, 30);
            this.hexagonControl1.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(4, 71);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 9;
            this.label3.Text = "Hexagon 3:";
            // 
            // hexagonControl3
            // 
            this.hexagonControl3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexagonControl3.Location = new System.Drawing.Point(72, 64);
            this.hexagonControl3.Name = "hexagonControl3";
            this.hexagonControl3.Size = new System.Drawing.Size(320, 30);
            this.hexagonControl3.TabIndex = 8;
            // 
            // HexPointControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.hexagonControl3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.hexagonControl2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hexagonControl1);
            this.Name = "HexPointControl";
            this.Size = new System.Drawing.Size(395, 100);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private HexagonControl hexagonControl2;
        private System.Windows.Forms.Label label1;
        private HexagonControl hexagonControl1;
        private System.Windows.Forms.Label label3;
        private HexagonControl hexagonControl3;
    }
}
