/* Author : Jonathan Borel-Jaquet
 * Date : 21/10/20
 * Description : Class Scene representing a store 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{
    public class Scene : Control
    {
        //Constant configuration
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
        private const int CHECKOUT_FIRST_POSITION_X = 50;
        private const int CHECKOUT_TIME_VERIFIER = 15;

        private Bitmap bitmap = null;
        private Graphics graphics = null;
        private readonly Timer timerDisplay;
        private readonly Timer timerSpawnClient;
        private readonly Timer timerCheckoutVerifier;
        private readonly Random random;

        internal List<Checkout> LstCheckout { get; private set; }
        internal List<Client> LstClient { get; private set; }
        public int TimeCheckoutVerifier { get; private set; }

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
            int x = CHECKOUT_FIRST_POSITION_X;
            //Creation of checkouts
            for (int i = 0; i < NUMBER_OF_CHECKOUT; i++)
            {
                Size size = new Size(CHECKOUT_WIDTH, CHECKOUT_HEIGHT);
                Vector2 position = new Vector2(x, CHECKOUT_POSITION_Y);
                Checkout checkout = new Checkout(position, size, this);
                Paint += checkout.Paint;
                timerDisplay.Tick += checkout.Tick;
                LstCheckout.Add(checkout);
                x += DISTANCE_BETWEEN_BOXES;
            }
            TimeCheckoutVerifier = CHECKOUT_TIME_VERIFIER;
        }
        /// <summary>
        /// Tick to refresh the display for drawing the different elements of the scene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Display_Tick(object sender, EventArgs e)
        {
            Invalidate();
        }
        /// <summary>
        /// Tick for spawning clients
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SpawnClient_Tick(object sender, EventArgs e)
        {
            if (LstClient.Count < MAX_CLIENT_IN_SHOP)
            {
                Vector2 position = new Vector2(CLIENT_START_POSITION_X, CLIENT_START_POSITION_Y);
                Vector2 speed = new Vector2(random.Next(50, 150), random.Next(50, 150));
                Size size = new Size(CLIENT_WIDTH, CLIENT_HEIGHT);
                Client client = new Client(position, new Vector2(1000, 1000), speed, size, this);
                Paint += client.Paint;
                timerDisplay.Tick += client.Tick;
                LstClient.Add(client);
            }

        }
        /// <summary>
        /// Verification tick to open checkouts if one or more customers are waiting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckoutVerifier_Tick(object sender, EventArgs e)
        {
            TimeCheckoutVerifier -= 1;
            if (TimeCheckoutVerifier == 0)
            {
                var clientsInCheckout = LstClient.Count(client => client.Status == Client.ClientStatus.WaitingQueue);
                if (clientsInCheckout > 0)
                {
                    var index = LstCheckout.TakeWhile(checkout => checkout.IsOpen).Count();
                    LstCheckout[index].OpenCheckout();
                }
                TimeCheckoutVerifier = CHECKOUT_TIME_VERIFIER;
            }
        }
        /// <summary>
        /// OnPaint method called by Invalidate()
        /// </summary>
        /// <param name="e"></param>
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

