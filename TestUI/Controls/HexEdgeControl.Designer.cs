namespace TestUI.Controls
{
    partial class HexEdgeControl
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
            this.hexagonControl1 = new TestUI.Controls.HexagonControl();
            this.label1 = new System.Windows.Forms.Label();
            this.hexagonControl2 = new TestUI.Controls.HexagonControl();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // hexagonControl1
            // 
            this.hexagonControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexagonControl1.Location = new System.Drawing.Point(73, 3);
            this.hexagonControl1.Name = "hexagonControl1";
            this.hexagonControl1.Size = new System.Drawing.Size(316, 30);
            this.hexagonControl1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Hexagon 1:";
            // 
            // hexagonControl2
            // 
            this.hexagonControl2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexagonControl2.Location = new System.Drawing.Point(73, 33);
            this.hexagonControl2.Name = "hexagonControl2";
            this.hexagonControl2.Size = new System.Drawing.Size(316, 30);
            this.hexagonControl2.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Hexagon 2:";
            // 
            // HexEdgeControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label2);
            this.Controls.Add(this.hexagonControl2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.hexagonControl1);
            this.Name = "HexEdgeControl";
            this.Size = new System.Drawing.Size(392, 66);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private HexagonControl hexagonControl1;
        private System.Windows.Forms.Label label1;
        private HexagonControl hexagonControl2;
        private System.Windows.Forms.Label label2;

    }
}
