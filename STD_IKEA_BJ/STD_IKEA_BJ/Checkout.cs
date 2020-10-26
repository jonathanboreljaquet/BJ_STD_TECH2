/* Author : Jonathan Borel-Jaquet
 * Date : 21/10/20
 * Description : Class Scene representing a store 
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{

    class Checkout
    {
        private const int MAX_NUMBER_OF_CLIENT_IN_CHECKOUT = 5;
        private const int START_TIME_BEFORE_PROCESS = 8;

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
        internal Queue<Client> ClientQueue { get; private set; }

        public Checkout(Vector2 position, Size size, Scene scene)
        {
            ClientQueue = new Queue<Client>();
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
        /// Opens the checkout.
        /// </summary>
        public void OpenCheckout()
        {
            color = Color.Green;
            IsOpen = true;
        }
        /// <summary>
        /// Closes the checkout
        /// </summary>
        public void CloseCheckout()
        {
            color = Color.Red;
            IsOpen = false;
        }
        /// <summary>
        /// Adds a client in the checkout
        /// </summary>
        /// <param name="client"></param>
        public void AddClientToQueue(Client client)
        {
            if (ClientQueue.Count < MAX_NUMBER_OF_CLIENT_IN_CHECKOUT)
            {
                ClientQueue.Enqueue(client);
            }
            IsFull = (ClientQueue.Count == MAX_NUMBER_OF_CLIENT_IN_CHECKOUT);

        }
        /// <summary>
        /// Removes the supported client and have the queue moved
        /// </summary>
        public void RemoveFirstClient()
        {
            if (ClientQueue.Count != 0)
            {
                Client removecClient = ClientQueue.Peek();
                removecClient.IsPainting = false;
                scene.ClientQueue.Dequeue();
                ClientQueue.Dequeue();
                actualQueuePositionY = position.Y;
                foreach (Client client in ClientQueue)
                {
                    Vector2 queuePosition = new Vector2(position.X, actualQueuePositionY);
                    client.Move(queuePosition);
                    actualQueuePositionY -= client.Size.Height;
                    IsFull = false;
                }
            }
            else
            {
                CloseCheckout();
            }        
        }
        /// <summary>
        /// Tick to add a client to the queue
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Tick(object sender, EventArgs e)
        {
            foreach (Client client in ClientQueue)
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
        /// Paints the checkout
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
