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
        private const int MAX_CLIENT_IN_SHOP = 7;
        private Bitmap bitmap = null;
        private Graphics graphics = null;
        private Timer timerShop;
        private Timer timerSpawnClient;
        private Checkout checkout;
        private Client client;
        private Random random;
        private int nbrClient;
        private List<Checkout> lstCheckout;
        private List<Client> lstClient;

        internal List<Checkout> LstCheckout { get => lstCheckout; set => lstCheckout = value; }
        internal List<Client> LstClient { get => lstClient; set => lstClient = value; }

        public Scene() : base()
        {
            lstCheckout = new List<Checkout>();
            lstClient = new List<Client>();
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
                checkout = new Checkout(new Vector2(x, 500));
                Paint += checkout.Paint;
                timerShop.Tick += checkout.Tick;
                lstCheckout.Add(checkout);
                x += 60;
            }
            lstCheckout[3].OpenCheckout();
        }

        private void Shop_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }
        private void SpawnClient_Tick(object sender, EventArgs e)
        {
            Vector2 speed = new Vector2(random.Next(50, 150), random.Next(50, 150));
            client = new Client(new Vector2(0, 0), speed, Color.White, this, new Size(40, 40));
            Paint += client.Paint;
            timerShop.Tick += client.Tick;
            lstClient.Add(client);
            nbrClient++;
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

