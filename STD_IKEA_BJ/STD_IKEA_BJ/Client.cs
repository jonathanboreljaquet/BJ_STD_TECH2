using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{
    class Client
    {
        private const int NO_SPEED_Y = 0;
        private Vector2 startPosition;
        private Vector2 actualPosition;
        private Vector2 speed;
        private Color color;
        private Scene scene;
        private Size size;
        private readonly Stopwatch sw;
        private bool isColliding;

        private Vector2 Location
        {
            get
            {
                float elapsedTime = sw.ElapsedMilliseconds / 1000f;
                Vector2 actualPosition = startPosition + elapsedTime * speed;
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
        public Client(Vector2 startPosition, Vector2 speed, Color color, Scene scene, Size size)
        {
            this.startPosition = startPosition;
            this.speed = speed;
            this.color = color;
            this.scene = scene;
            this.size = size;
            sw = new Stopwatch();
            sw.Start();
        }
        public void Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillEllipse(new SolidBrush(color), new Rectangle(Point.Round(Location.ToPointF()), size));

        }
    }
}
