using System;
using System.Windows.Forms;
using TrackerLibrary;

namespace TrackerUI
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Initialize the database connections
            GlobalConfig.InitializeConnections(true, true);

            Application.Run(new CreatePrizeForm());
            //Application.Run(new TournamentDashboardForm());
        }
    }
}
