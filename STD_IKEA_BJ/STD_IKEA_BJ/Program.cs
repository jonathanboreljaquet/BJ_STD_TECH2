/* Author : Jonathan Borel-Jaquet
 * Date : 21/10/20
 * Description : Program.cs file
 */
using System;
using System.Windows.Forms;

namespace STD_IKEA_BJ
{
    static class Program
    {
        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new frmIkea());
        }
    }
}
