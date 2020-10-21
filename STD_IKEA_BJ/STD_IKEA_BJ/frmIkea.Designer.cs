namespace STD_IKEA_BJ
{
    partial class frmIkea
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

        #region Code généré par le Concepteur Windows Form

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.scene1 = new STD_IKEA_BJ.Scene();
            this.magasinInfo1 = new STD_IKEA_BJ.MagasinInfo();
            this.SuspendLayout();
            // 
            // scene1
            // 
            this.scene1.BackColor = System.Drawing.Color.Silver;
            this.scene1.Location = new System.Drawing.Point(13, 13);
            this.scene1.Name = "scene1";
            this.scene1.Size = new System.Drawing.Size(950, 577);
            this.scene1.TabIndex = 0;
            this.scene1.Text = "scene1";
            // 
            // magasinInfo1
            // 
            this.magasinInfo1.Location = new System.Drawing.Point(732, 434);
            this.magasinInfo1.Name = "magasinInfo1";
            this.magasinInfo1.Scene = this.scene1;
            this.magasinInfo1.Size = new System.Drawing.Size(214, 140);
            this.magasinInfo1.TabIndex = 1;
            // 
            // frmIkea
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(975, 602);
            this.Controls.Add(this.magasinInfo1);
            this.Controls.Add(this.scene1);
            this.Name = "frmIkea";
            this.Text = "IKEA";
            this.ResumeLayout(false);

        }

        #endregion

        private Scene scene1;
        private MagasinInfo magasinInfo1;
    }
}

