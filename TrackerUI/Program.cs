using System;
using System.Windows.Forms;
using TournamentTracker;
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
            GlobalConfig.InitializeConnections(DataAccessType.SqlAccess);

            Application.Run(new TournamentDashboardForm());
        }
    }
}
