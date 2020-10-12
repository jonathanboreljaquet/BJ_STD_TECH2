using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{
    public partial class MagasinInfo : UserControl
    {
        public Scene Scene { get; set; }
        public MagasinInfo()
        {    
            InitializeComponent();
        }

        private void MagasinInfo_Paint(object sender, PaintEventArgs e)
        {
            if (Scene!=null)
            {
                lblNbrCheckoutOpen.Text = Scene.LstCheckout.Count(n => n.IsOpen == true).ToString()+"/"+Scene.LstCheckout.Count.ToString() ;

            }
        }
    }
}
