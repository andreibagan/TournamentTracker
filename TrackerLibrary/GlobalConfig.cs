using System.Configuration;
using System.IO;
using TournamentTracker;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        public static IDataConnection Connection { get; private set; }
        private static ISqlDataAccess _sqlDataAccess = new SqlDataAccess();
        private static ITextFileDataAccess _textDataAccess = new TextFileDataAccess();

        public static void InitializeConnections(DataAccessType dataAccessType)
        {
            if (dataAccessType == DataAccessType.SqlAccess)
            {
                Connection = new SqlConnector(_sqlDataAccess);
            }
            else if (dataAccessType == DataAccessType.FileAccess)
            {
                Connection = new TextConnector(_textDataAccess);
            }
        }

        public static string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }

        public static string GetFullFilePath(string fileName)
        {
            return Path.Combine(ConfigurationManager.AppSettings["filePath"], fileName);
        }
    }
}
