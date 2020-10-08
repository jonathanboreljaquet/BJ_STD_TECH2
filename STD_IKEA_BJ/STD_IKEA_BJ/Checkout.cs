using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{
    class Checkout
    {
        const int MAX_NUMBER_OF_CLIENT = 5;
        private const int START_ELPAPSED_TIME = 20;
        private const int DEFAULT_TIME_PROCESS = 20;
        private Vector2 position;
        private List<Client> lstClient;
        private Timer timer;
        private int timeElapsed;
        private string label;
        private Color color;
        public bool IsOpen { get; private set; }
        public bool IsFull { get; private set; }
        public Vector2 Position { get => position; private set => position = value; }
        internal List<Client> LstClient { get => lstClient; private set => lstClient = value; }

        public Checkout(Vector2 position)
        {
            lstClient = new List<Client>();
            timer = new Timer
            {
                Interval = 1000,
                Enabled = true
            };
            timer.Tick += T_Tick;
            this.position = position;
            label = "-";
            color = Color.Red;
            timeElapsed = DEFAULT_TIME_PROCESS;
        }

        private void T_Tick(object sender, EventArgs e)
        {
            if (IsOpen)
            {
                timeElapsed -= 1;
                if (timeElapsed < 0)
                {
                    timeElapsed = START_ELPAPSED_TIME;
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
            if (lstClient.Count < MAX_NUMBER_OF_CLIENT)
            {
                lstClient.Add(client);
            }
            else
            {
                IsFull = true;
            }
            
        }

        public void Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(color), new Rectangle(Point.Round(position.ToPointF()), new Size(50, 50)));
            e.Graphics.DrawString(label, new Font("arial", 11F), Brushes.Black, (float)position.X, (float)position.Y);
        }
    }
}
