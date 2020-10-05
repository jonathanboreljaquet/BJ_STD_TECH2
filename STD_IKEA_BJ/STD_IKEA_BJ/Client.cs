using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{
    class Client
    {
        private float elapsedTime;
        private PointF startPosition;
        private PointF speed;
        private Color color;
        private Scene scene;
        private readonly Stopwatch sw;

        private PointF Location
        {
            get
            {
                PointF newLocation;
                elapsedTime = sw.ElapsedMilliseconds / 1000f;
                newLocation = new PointF(startPosition.X + elapsedTime * speed.X, startPosition.Y + elapsedTime * speed.Y);
                if (newLocation.X-40>scene.Width)
                {
                    newLocation = new PointF(startPosition.X + elapsedTime * (speed.X*-1), startPosition.Y + elapsedTime * speed.Y);
                }
                return newLocation;
            }

        }
        public Client(PointF startPosition, PointF speed, Color color, Scene scene)
        {
            this.startPosition = startPosition;
            this.speed = speed;
            this.color = color;
            this.scene = scene;
            sw = new Stopwatch();
            sw.Start();
        }
        public void Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillEllipse(new SolidBrush(color), new Rectangle(Point.Round(Location), new Size(40, 40)));

        }
    }
}
