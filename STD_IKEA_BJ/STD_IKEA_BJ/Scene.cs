using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{
    public class Scene : Control
    {
        private const int FPS = 120;
        private const int NUMBER_OF_CHECKOUT = 10;
        private const int CHECKOUT_POSITION_Y = 500;
        private const int DISTANCE_BETWEEN_BOXES = 60;
        private const int CHECKOUT_WIDTH = 50;
        private const int CHECKOUT_HEIGHT = 50;
        private const int CLIENT_WIDTH = 40;
        private const int CLIENT_HEIGHT = 40;
        private const int CLIENT_START_POSITION_X = 0;
        private const int CLIENT_START_POSITION_Y = 0;
        private const int MAX_CLIENT_IN_SHOP = 7;
        private Bitmap bitmap = null;
        private Graphics graphics = null;
        private Timer timerShop;
        private Timer timerSpawnClient;
        private Checkout checkout;
        private Random random;
        private int nbrClient = 0;

        internal List<Checkout> LstCheckout { get; private set; }
        internal List<Client> LstClient { get; private set; }

        public Scene() : base()
        {
            LstCheckout = new List<Checkout>();
            LstClient = new List<Client>();
            random = new Random();
            DoubleBuffered = true;
            timerShop = new Timer
            {
                Interval = 1000 / FPS,
                Enabled = true
            };
            timerShop.Tick += Shop_Tick;
            timerSpawnClient = new Timer
            {
                Interval = 1000,
                Enabled = true
            };
            timerSpawnClient.Tick += SpawnClient_Tick;
            int x = 50;
            for (int i = 0; i < NUMBER_OF_CHECKOUT; i++)
            {
                Size size = new Size(CHECKOUT_WIDTH, CHECKOUT_HEIGHT);
                Vector2 position = new Vector2(x, CHECKOUT_POSITION_Y);
                checkout = new Checkout(position, size, this);
                Paint += checkout.Paint;
                timerShop.Tick += checkout.Tick;
                LstCheckout.Add(checkout);
                x += DISTANCE_BETWEEN_BOXES;
            }
            LstCheckout[0].OpenCheckout();
        }

        private void Shop_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }
        private void SpawnClient_Tick(object sender, EventArgs e)
        {
            if (nbrClient < 2)
            {
                Vector2 position = new Vector2(CLIENT_START_POSITION_X, CLIENT_START_POSITION_Y);
                Vector2 speed = new Vector2(random.Next(50, 150), random.Next(50, 150));
                Size size = new Size(CLIENT_WIDTH,CLIENT_HEIGHT);
                Client client = new Client(position, new Vector2(), speed, Color.White, size, this);
                Paint += client.Paint;
                timerShop.Tick += client.Tick;
                LstClient.Add(client);
                nbrClient++;
            }

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

