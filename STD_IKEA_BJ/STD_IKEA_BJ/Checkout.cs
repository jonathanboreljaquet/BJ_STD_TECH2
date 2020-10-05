using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{
    class Checkout
    {
        const int MAX_NUMBER_OF_CLIENT = 5;
        private const int START_ELPAPSED_TIME = 20;
        private Point position;
        private List<Client> lstClient;
        private Timer timer;
        private int timeElapsed;
        public Checkout(Point position)
        {
            timeElapsed = START_ELPAPSED_TIME;
            timer = new Timer
            {
                Interval = 1000,

                Enabled = true
            };
            timer.Tick += T_Tick;
            this.position = position;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            timeElapsed -= 1;
            if (timeElapsed<0)
            {
                timeElapsed = START_ELPAPSED_TIME;
            }
        }

        public void Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(Color.Red),new Rectangle(position,new Size(50,50)));
            e.Graphics.DrawString(timeElapsed.ToString(), new Font("arial", 11F), Brushes.Black, position);
                
        }
    }
}
