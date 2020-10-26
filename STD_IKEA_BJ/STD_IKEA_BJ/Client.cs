/* Author : Jonathan Borel-Jaquet
 * Date : 21/10/20
 * Description : Class Client representing a client in the store
 */
using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{
    class Client
    {
        private const int MINIMUM_CLIENT_TIME_BEFORE_PURCHASE = 10;
        private const int MAXIMUM_CLIENT_TIME_BEFORE_PURCHASE = 30;

        private Vector2 startPosition;
        private Vector2 actualPosition;
        private Vector2 futurPosition;
        private Vector2 speed;
        private Color color;
        private Scene scene;
        private Size size;
        private bool isColliding;
        private int timePurchase;

        private readonly Timer timerPurchase;
        private readonly Random rdm;
        private readonly Stopwatch sw;

        public bool IsInCheckout { get; private set; }
        public bool IsPainting { get; set; }
        public Size Size { get => size; private set => size = value; }
        public Vector2 ActualPosition { get => actualPosition; private set => actualPosition = value; }
        public ClientStatus Status { get; private set; }
        public enum ClientStatus
        {
            Walking = 0,
            WaitingQueue = 1,
            InQueue = 2
        }
        private Vector2 Location
        {
            get
            {
                float elapsedTime = sw.ElapsedMilliseconds / 1000f;
                actualPosition = startPosition + elapsedTime * speed;
                //Checking that the collision trigger not multiple times
                bool oldColliding = isColliding;
                isColliding = (actualPosition.X + size.Width >= scene.Width || actualPosition.X <= 0 || actualPosition.Y + size.Height >= scene.Height || actualPosition.Y <= 0);
                if ((actualPosition.X + size.Width >= scene.Width || actualPosition.X <= 0) && !oldColliding && isColliding)
                {
                    startPosition = actualPosition;
                    speed.X = -speed.X;
                    sw.Restart();
                }
                if ((actualPosition.Y + size.Height >= scene.Height || actualPosition.Y <= 0) && !oldColliding && isColliding)
                {
                    startPosition = actualPosition;
                    speed.Y = -speed.Y;
                    sw.Restart();
                }
                return actualPosition;
            }
        }


        public Client(Vector2 startPosition, Vector2 futurPosition, Vector2 speed, Size size, Scene scene)
        {
            this.startPosition = startPosition;
            this.futurPosition = futurPosition;
            this.speed = speed;
            this.size = size;
            this.scene = scene;
            IsPainting = true;
            sw = new Stopwatch();
            sw.Start();
            timerPurchase = new Timer
            {
                Interval = 1000,
                Enabled = true
            };
            timerPurchase.Tick += BuyingTime_Tick;
            rdm = new Random();
            timePurchase = rdm.Next(MINIMUM_CLIENT_TIME_BEFORE_PURCHASE, MAXIMUM_CLIENT_TIME_BEFORE_PURCHASE);
            Status = ClientStatus.Walking;
        }
        /// <summary>
        /// Tick to get a customer through the checkout process when they need it.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BuyingTime_Tick(object sender, EventArgs e)
        {
            if (timePurchase == 0 || Status == ClientStatus.WaitingQueue)
            {
                foreach (Checkout checkout in scene.LstCheckout)
                {
                    if (Status == ClientStatus.Walking)
                    {
                        Status = ClientStatus.WaitingQueue;
                    }
                    if (checkout.IsOpen && !checkout.IsFull)
                    {
                        if (!IsInCheckout)
                        {
                            this.Status = ClientStatus.InQueue;
                            checkout.AddClientToQueue(this);
                            return;
                        }
                    }
                }
            }
            timePurchase -= 1;

        }
        /// <summary>
        /// Moves the client to a desired destination
        /// </summary>
        /// <param name="destination"></param>
        public void Move(Vector2 destination)
        {
            futurPosition = destination;
            startPosition = actualPosition;
            float diffx = actualPosition.X - futurPosition.X;
            float diffy = actualPosition.Y - futurPosition.Y;
            speed = new Vector2(diffx * -1, diffy * -1);
            IsInCheckout = true;
            sw.Restart();
        }
        /// <summary>
        /// Stops the client's movement
        /// </summary>
        public void Stop()
        {
            startPosition = actualPosition;
            speed = new Vector2(0, 0);
            sw.Restart();
        }
        /// <summary>
        /// Client's Paint method used by the OnPaint of the Scene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Paint(object sender, PaintEventArgs e)
        {
            if (IsPainting)
            {
                e.Graphics.FillEllipse(new SolidBrush(color), new Rectangle(Point.Round(Location.ToPointF()), size));
                if (Status == ClientStatus.Walking)
                {
                    e.Graphics.DrawString(timePurchase.ToString(), new Font("arial", 11F), Brushes.Black, (float)Location.X, (float)Location.Y);
                }
            }

        }
        /// <summary>
        /// Client Tick method used by the DisplayTick of the scene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Tick(object sender, EventArgs e)
        {
            if ((actualPosition.Y > futurPosition.Y) &&
                (actualPosition.Y < futurPosition.Y + (size.Height / 2)) &&
                (actualPosition.X < futurPosition.X + (size.Width / 2)))
            {
                Stop();
            }
            switch (Status)
            {
                case ClientStatus.Walking:
                    color = Color.White;
                    break;
                case ClientStatus.WaitingQueue:
                    color = Color.Red;
                    break;
                case ClientStatus.InQueue:
                    color = Color.Yellow;
                    break;
                default:
                    break;
            }

        }



    }
}
