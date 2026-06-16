using System;
using System.Windows.Forms;
using ConvenienceStoreApp.Forms;

namespace ConvenienceStoreApp
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            // Start the application by showing the Login Form
            Application.Run(new LoginForm());
        }
    }
}
