namespace STD_IKEA_BJ
{
    partial class MagasinInfo
    {
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lblNbrCheckoutOpen = new System.Windows.Forms.Label();
            this.lblTimeBeforeOpenCheckout = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.lblNumberOfClientsInShop = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Caisses ouvertes";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 41);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(179, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Temps avant vérification d\'ouverture";
            // 
            // lblNbrCheckoutOpen
            // 
            this.lblNbrCheckoutOpen.AutoSize = true;
            this.lblNbrCheckoutOpen.Location = new System.Drawing.Point(96, 16);
            this.lblNbrCheckoutOpen.Name = "lblNbrCheckoutOpen";
            this.lblNbrCheckoutOpen.Size = new System.Drawing.Size(10, 13);
            this.lblNbrCheckoutOpen.TabIndex = 2;
            this.lblNbrCheckoutOpen.Text = "-";
            // 
            // lblTimeBeforeOpenCheckout
            // 
            this.lblTimeBeforeOpenCheckout.AutoSize = true;
            this.lblTimeBeforeOpenCheckout.Location = new System.Drawing.Point(188, 41);
            this.lblTimeBeforeOpenCheckout.Name = "lblTimeBeforeOpenCheckout";
            this.lblTimeBeforeOpenCheckout.Size = new System.Drawing.Size(10, 13);
            this.lblTimeBeforeOpenCheckout.TabIndex = 4;
            this.lblTimeBeforeOpenCheckout.Text = "-";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(171, 13);
            this.label3.TabIndex = 5;
            this.label3.Text = "Nombre de clients dans le magasin";
            // 
            // lblNumberOfClientsInShop
            // 
            this.lblNumberOfClientsInShop.AutoSize = true;
            this.lblNumberOfClientsInShop.Location = new System.Drawing.Point(180, 64);
            this.lblNumberOfClientsInShop.Name = "lblNumberOfClientsInShop";
            this.lblNumberOfClientsInShop.Size = new System.Drawing.Size(10, 13);
            this.lblNumberOfClientsInShop.TabIndex = 6;
            this.lblNumberOfClientsInShop.Text = "-";
            // 
            // MagasinInfo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.lblNumberOfClientsInShop);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lblTimeBeforeOpenCheckout);
            this.Controls.Add(this.lblNbrCheckoutOpen);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Name = "MagasinInfo";
            this.Size = new System.Drawing.Size(228, 150);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.MagasinInfo_Paint);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblNbrCheckoutOpen;
        private System.Windows.Forms.Label lblTimeBeforeOpenCheckout;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblNumberOfClientsInShop;
    }
}
