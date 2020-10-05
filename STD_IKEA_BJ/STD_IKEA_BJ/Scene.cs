using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{
    class Scene : Control
    {
        private const int FPS = 60;
        private const int NUMBER_OF_CHECKOUT = 10;
        private const int MAX_CLIENT_IN_SHOP = 50;
        private Bitmap bitmap = null;
        private Graphics graphics = null;
        private Timer timer;
        private Checkout checkout;
        private Client client;
        private Random random;
        private int tick;

        public Scene() :base()
        {
            DoubleBuffered = true;
            timer = new Timer
            {
                Interval = 1000 / FPS,
                Enabled = true
            };
            timer.Tick += T_Tick;
            int x = 50;
            for (int i = 0; i < NUMBER_OF_CHECKOUT; i++)
            {
                checkout = new Checkout(new Point(x, 500));
                Paint += checkout.Paint;
                x += 60;
            }
            random = new Random();



        }

        private void T_Tick(object sender, EventArgs e)
        {
            
            if (tick%50==0)
            {
                PointF speed = new PointF(random.Next(50, 150), random.Next(50, 150));
                client = new Client(new PointF(0, 0), speed, Color.White, this);
                Paint += client.Paint;
            }
            tick += 1;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            // Initialize bitmap and g if null
            bitmap ??= new Bitmap(Size.Width, Size.Height);
            graphics ??= Graphics.FromImage(bitmap);
            graphics.SmoothingMode = SmoothingMode.AntiAlias;
            PaintEventArgs p = new PaintEventArgs(graphics, e.ClipRectangle);
            p.Graphics.Clear(BackColor);
            base.OnPaint(p);
            e.Graphics.DrawImage(bitmap, new Point(0, 0));
        }
    }
}

