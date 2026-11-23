using System;
using System.Windows.Forms;

namespace Interfaz
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación Windows Forms de NEXUS.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using (FrmSplash splash = new FrmSplash())
            {
                splash.ShowDialog();
            }

            Application.Run(new Form1());
        }
    }
}