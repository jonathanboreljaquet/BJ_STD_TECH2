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
        private const int MAX_CLIENT_IN_SHOP = 20;
        private Bitmap bitmap = null;
        private Graphics graphics = null;
        private Timer timerDisplay;
        private Timer timerSpawnClient;
        private Timer timerCheckoutVerifier;
        private int timeCheckoutVerifier;
        private Checkout checkout;
        private Client client;
        private Random random;

        internal List<Checkout> LstCheckout { get; private set; }
        internal List<Client> LstClient { get; private set; }
        public int TimeCheckoutVerifier { get => timeCheckoutVerifier; set => timeCheckoutVerifier = value; }

        public Scene() : base()
        {
            LstCheckout = new List<Checkout>();
            LstClient = new List<Client>();
            random = new Random();
            DoubleBuffered = true;
            timerDisplay = new Timer
            {
                Interval = 1000 / FPS,
                Enabled = true
            };
            timerDisplay.Tick += Display_Tick;
            timerSpawnClient = new Timer
            {
                Interval = 1000,
                Enabled = true
            };
            timerSpawnClient.Tick += SpawnClient_Tick;
            timerCheckoutVerifier = new Timer
            {
                Interval = 1000,
                Enabled = true
            };
            timerCheckoutVerifier.Tick += CheckoutVerifier_Tick;
            int x = 50;
            for (int i = 0; i < NUMBER_OF_CHECKOUT; i++)
            {
                Size size = new Size(CHECKOUT_WIDTH, CHECKOUT_HEIGHT);
                Vector2 position = new Vector2(x, CHECKOUT_POSITION_Y);
                checkout = new Checkout(position, size, this);
                Paint += checkout.Paint;
                timerDisplay.Tick += checkout.Tick;
                LstCheckout.Add(checkout);
                x += DISTANCE_BETWEEN_BOXES;
            }
            timeCheckoutVerifier = 15;
        }
        private void Display_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }
        private void SpawnClient_Tick(object sender, EventArgs e)
        {
            if (LstClient.Count < MAX_CLIENT_IN_SHOP)
            {
                Vector2 position = new Vector2(CLIENT_START_POSITION_X, CLIENT_START_POSITION_Y);
                Vector2 speed = new Vector2(random.Next(50, 150), random.Next(50, 150));
                Size size = new Size(CLIENT_WIDTH, CLIENT_HEIGHT);
                client = new Client(position, new Vector2(1000, 1000), speed, size, this);
                Paint += client.Paint;
                timerDisplay.Tick += client.Tick;
                LstClient.Add(client);
            }

        }
        private void CheckoutVerifier_Tick(object sender, EventArgs e)
        {
            timeCheckoutVerifier -= 1;
            if (timeCheckoutVerifier == 0)
            {
                var clientsInCheckout = LstClient.Count(client => client.IsInCheckout == false);
                if (clientsInCheckout > 0)
                {
                    var index = LstCheckout.TakeWhile(checkout => checkout.IsOpen).Count();
                    LstCheckout[index].OpenCheckout();
                }
                timeCheckoutVerifier = 15;
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

