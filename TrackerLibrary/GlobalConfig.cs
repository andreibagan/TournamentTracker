using System.Collections.Generic;
using System.Configuration;
using System.IO;
using TournamentTracker.DataAccess;
using TrackerLibrary.DataAccess;

namespace TrackerLibrary
{
    public static class GlobalConfig
    {
        public static List<IDataConnection> Connections { get; private set; } = new List<IDataConnection>();
        private static ISqlDataAccess _db = new SqlDataAccess();

        public static void InitializeConnections(bool database, bool textFiles)
        {
            if (database)
            {
                SqlConnector sql = new SqlConnector(_db);
                Connections.Add(sql);
            }

            if (textFiles)
            {
                TextConnector text = new TextConnector(new TextFileDataAccess());
                Connections.Add(text);
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
