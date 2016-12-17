namespace TestUI.Controls
{
    partial class TradeOfferControl
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
            this.playerId = new TestUI.Controls.IntegerControl();
            this.toGive = new TestUI.Controls.ResourceStackControl();
            this.toReceive = new TestUI.Controls.ResourceStackControl();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // playerId
            // 
            this.playerId.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.playerId.Location = new System.Drawing.Point(118, 3);
            this.playerId.Name = "playerId";
            this.playerId.Size = new System.Drawing.Size(252, 21);
            this.playerId.TabIndex = 0;
            // 
            // toGive
            // 
            this.toGive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toGive.Location = new System.Drawing.Point(115, 28);
            this.toGive.Name = "toGive";
            this.toGive.Size = new System.Drawing.Size(258, 28);
            this.toGive.TabIndex = 1;
            // 
            // toReceive
            // 
            this.toReceive.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.toReceive.Location = new System.Drawing.Point(115, 56);
            this.toReceive.Name = "toReceive";
            this.toReceive.Size = new System.Drawing.Size(258, 28);
            this.toReceive.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(1, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Player Id";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(1, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(93, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Resources to give";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(1, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(108, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Resources to receive";
            // 
            // TradeOfferControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.toReceive);
            this.Controls.Add(this.toGive);
            this.Controls.Add(this.playerId);
            this.Name = "TradeOfferControl";
            this.Size = new System.Drawing.Size(373, 83);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private IntegerControl playerId;
        private ResourceStackControl toGive;
        private ResourceStackControl toReceive;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}
