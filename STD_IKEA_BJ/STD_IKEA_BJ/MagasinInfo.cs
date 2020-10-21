/* Author : Jonathan Borel-Jaquet
 * Date : 21/10/20
 * Description : Class MagasinInfo allowing to display store information
 */
using System;
using System.Linq;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{
    public partial class MagasinInfo : UserControl
    {
        public Scene Scene { get; set; }
        private Timer timerDisplay;
        public MagasinInfo()
        {    
            InitializeComponent();
            timerDisplay = new Timer
            {
                Interval = 1000,
                Enabled = true
            };
            timerDisplay.Tick += Display_Tick;
        }
        /// <summary>
        /// Tick to refresh the display for drawing the different information of the shop
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }
        /// <summary>
        /// Paint method called by Invalidate()
        /// </summary>
        /// <param name="e"></param>
        private void MagasinInfo_Paint(object sender, PaintEventArgs e)
        {
            if (Scene!=null)
            {
                lblNbrCheckoutOpen.Text = Scene.LstCheckout.Count(n => n.IsOpen == true).ToString()+"/"+Scene.LstCheckout.Count.ToString() ;
                lblTimeBeforeOpenCheckout.Text = Scene.TimeCheckoutVerifier.ToString();
                lblNumberOfClientsInShop.Text = Scene.LstClient.Count.ToString();
            }
        }
    }
}
