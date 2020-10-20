using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;
using System.Linq;

namespace STD_IKEA_BJ
{
    class Client
    {
        private Vector2 startPosition;
        private Vector2 actualPosition;
        private Vector2 futurPosition;
        private Vector2 speed;
        private Color color;
        private Scene scene;
        private Size size;
        private readonly Stopwatch sw;
        private bool isColliding;
        private Timer timerPurchase;
        private int timePurchase;
        private Random rdm;
        private ClientStatus status;

        public bool IsInCheckout { get; private set; }
        public bool IsPainting { get; set; }
        public Size Size { get => size; private set => size = value; }
        public Vector2 ActualPosition { get => actualPosition; private set => actualPosition = value; }
        private enum ClientStatus
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
            timePurchase = rdm.Next(10, 30);
            status = ClientStatus.Walking;
        }

        private void BuyingTime_Tick(object sender, EventArgs e)
        {
            timePurchase -= 1;
            if (timePurchase == 0 || status == ClientStatus.WaitingQueue)
            {
                status = ClientStatus.WaitingQueue;
                foreach (Checkout checkout in scene.LstCheckout)
                {
                    if (checkout.IsOpen && !checkout.IsFull)
                    {
                        if (!IsInCheckout)
                        {
                            status = ClientStatus.InQueue;
                            checkout.AddClientToQueue(this);
                        }
                    }
                }
                timerPurchase.Enabled = false;
            }
            
        }

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
        public void Stop()
        {
            startPosition = actualPosition;
            speed = new Vector2(0, 0);
            sw.Restart();
        }
        public void Paint(object sender, PaintEventArgs e)
        {
            if (IsPainting)
            {
                e.Graphics.FillEllipse(new SolidBrush(color), new Rectangle(Point.Round(Location.ToPointF()), size));
                if (!IsInCheckout)
                {
                    e.Graphics.DrawString(timePurchase.ToString(), new Font("arial", 11F), Brushes.Black, (float)Location.X, (float)Location.Y);
                }
            }

        }
        public void Tick(object sender, EventArgs e)
        {
            if (actualPosition.Y > futurPosition.Y && actualPosition.Y < futurPosition.Y + size.Height && actualPosition.X < futurPosition.X + (size.Width / 2))
            {
                Stop();
            }
            switch (status)
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
