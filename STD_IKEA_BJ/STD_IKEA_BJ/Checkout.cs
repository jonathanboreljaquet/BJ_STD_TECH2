using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{

    class Checkout
    {
        const int MAX_NUMBER_OF_CLIENT = 3;
        private const int START_ELPAPSED_TIME = 5;
        private Vector2 position;
        private Size size;
        private float actualQueuePositionY;
        private Scene scene;
        private Timer timer;
        private int timeElapsed;
        private string label;
        private Color color;
        public bool IsOpen { get; private set; }
        public bool IsFull { get; private set; }
        internal List<Client> LstClient { get; private set; }

        public Checkout(Vector2 position,Size size, Scene scene)
        {
            LstClient = new List<Client>();
            timer = new Timer
            {
                Interval = 1000,
                Enabled = true
            };
            timer.Tick += T_Tick;
            this.position = position;
            this.size = size;
            this.actualQueuePositionY = position.Y;
            this.scene = scene;
            label = "-";
            color = Color.Red;
            timeElapsed = START_ELPAPSED_TIME;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (IsOpen)
            {
                timeElapsed -= 1;
                if (timeElapsed < 0)
                {
                    timeElapsed = START_ELPAPSED_TIME;
                    RemoveClient();
                }
                label = timeElapsed.ToString();
            }
        }
        public void OpenCheckout()
        {
            color = Color.Green;
            IsOpen = true;
        }
        public void AddClientToQueue(Client client)
        {
            if (LstClient.Count < MAX_NUMBER_OF_CLIENT)
            {
                LstClient.Add(client);
            }
            else
            {
                IsFull = true;
            }

        }
        public void RemoveClient()
        {
            actualQueuePositionY = position.Y;
            LstClient[0].IsPainting = false;
            LstClient[0] = null;
            scene.LstClient[0] = null;
            scene.LstClient.Remove(LstClient[0]);
            LstClient.Remove(LstClient[0]);
            foreach (Client client in LstClient)
            {
                Vector2 queuePosition = new Vector2(position.X, actualQueuePositionY);
                client.Move(queuePosition);
                actualQueuePositionY -= client.Size.Height;
                IsFull = false;
            }
        }
        public void Tick(object sender, EventArgs e)
        {
            foreach (Client client in LstClient)
            {
                if (!client.IsInCheckout)
                {
                    Vector2 queuePosition = new Vector2(position.X, actualQueuePositionY);
                    client.Move(queuePosition);
                    actualQueuePositionY -= client.Size.Height;
                }
            }
        }

        public void Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(color), new Rectangle(Point.Round(position.ToPointF()), size));
            e.Graphics.DrawString(label, new Font("arial", 11F), Brushes.Black, (float)position.X, (float)position.Y);
        }
    }
}
