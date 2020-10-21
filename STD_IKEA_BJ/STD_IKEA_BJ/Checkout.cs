/* Author : Jonathan Borel-Jaquet
 * Date : 21/10/20
 * Description : Class Scene representing a store 
 */
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
        private const int START_TIME_BEFORE_PROCESS = 10;
        private Vector2 position;
        private Size size;
        private float actualQueuePositionY;
        private readonly Scene scene;
        private readonly Timer timerProcessing;
        private int timeBeforeProcess;
        private string label;
        private Color color;
        public bool IsOpen { get; private set; }
        public bool IsFull { get; private set; }
        internal List<Client> LstClient { get; private set; }

        public Checkout(Vector2 position,Size size, Scene scene)
        {
            LstClient = new List<Client>();
            timerProcessing = new Timer
            {
                Interval = 1000,
                Enabled = true
            };
            timerProcessing.Tick += Processing_Tick;
            this.position = position;
            this.size = size;
            this.actualQueuePositionY = position.Y;
            this.scene = scene;
            label = "close";
            color = Color.Red;
            timeBeforeProcess = START_TIME_BEFORE_PROCESS;
        }
        /// <summary>
        /// Tick to process a client 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Processing_Tick(object sender, EventArgs e)
        {
            if (IsOpen)
            {
                timeBeforeProcess -= 1;
                if (timeBeforeProcess == 0)
                {
                    timeBeforeProcess = START_TIME_BEFORE_PROCESS;
                    RemoveFirstClient();
                }
                label = timeBeforeProcess.ToString();
            }
        }
        /// <summary>
        /// Method for open the checkout
        /// </summary>
        public void OpenCheckout()
        {
            color = Color.Green;
            IsOpen = true;
        }
        /// <summary>
        /// Method for add a client in the checkout
        /// </summary>
        /// <param name="client"></param>
        public void AddClientToQueue(Client client)
        {
            if (LstClient.Count < MAX_NUMBER_OF_CLIENT)
            {
                LstClient.Add(client);
            }
            IsFull = (LstClient.Count==MAX_NUMBER_OF_CLIENT);

        }
        /// <summary>
        /// Method to remove the supported client and have the queue moved
        /// </summary>
        public void RemoveFirstClient()
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
        /// <summary>
        /// Tick to add a client to the queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
        /// <summary>
        /// Checkout Paint method used by the OnPaint of the Scene
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.FillRectangle(new SolidBrush(color), new Rectangle(Point.Round(position.ToPointF()), size));
            e.Graphics.DrawString(label, new Font("arial", 11F), Brushes.Black, (float)position.X, (float)position.Y);
        }
    }
}
