using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Background
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isFirst;
            new System.Threading.Mutex(false, System.Reflection.Assembly.
                GetExecutingAssembly().GetType().GUID.ToString(),
                out isFirst);
            if (!isFirst)
            {
                MessageBox.Show("Програма вже запущена");
                Environment.Exit(0);
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new AppContext());
        }
    }
}
