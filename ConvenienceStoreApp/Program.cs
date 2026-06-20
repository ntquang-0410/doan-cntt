using System;
using System.Threading;
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
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            
            // Start the application by showing the Login Form
            Application.Run(new LoginForm());
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(
                "Ứng dụng gặp lỗi giao diện:\n" + e.Exception.Message,
                "Lỗi ứng dụng",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            MessageBox.Show(
                "Ứng dụng gặp lỗi không mong muốn:\n" + (ex != null ? ex.Message : e.ExceptionObject.ToString()),
                "Lỗi ứng dụng",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
