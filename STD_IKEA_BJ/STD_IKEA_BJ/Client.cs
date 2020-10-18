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
        private const int NO_SPEED_Y = 0;
        private Vector2 startPosition;
        private Vector2 actualPosition;
        private Vector2 futurPosition;
        private Vector2 speed;
        private Color color;
        private Scene scene;
        private Size size;
        private readonly Stopwatch sw;
        private bool isColliding;

        public bool IsInCheckout { get; private set; }
        public bool IsPainting { get; set; }

        public Size Size { get => size; private set => size = value; }
        public Vector2 ActualPosition { get => actualPosition; private set => actualPosition = value; }


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


        public Client(Vector2 startPosition, Vector2 futurPosition, Vector2 speed, Color color, Size size, Scene scene)
        {
            this.startPosition = startPosition;
            this.futurPosition = futurPosition;
            this.speed = speed;
            this.color = color;
            this.size = size;
            this.scene = scene;
            IsPainting = true;
            sw = new Stopwatch();
            sw.Start();
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
            }

        }
        private int cpt = 0;
        public void Tick(object sender, EventArgs e)
        {
            foreach (Checkout checkout in scene.LstCheckout)
            {
                if (checkout.IsOpen && !checkout.IsFull)
                {
                    if (!IsInCheckout)
                    {
                        checkout.AddClientToQueue(this);
                    }
                }
            }
            if (actualPosition.Y > futurPosition.Y && actualPosition.X>futurPosition.X)
            {
                Stop();
                Console.WriteLine("Test"+cpt.ToString());
                cpt++;
            }

        }
        


    }
}
