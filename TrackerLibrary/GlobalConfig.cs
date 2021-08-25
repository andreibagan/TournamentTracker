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
        public const string PrizesFileName = "PrizeModels.csv";
        public const string PeopleFileName = "PersonModels.csv";
        public const string TeamsFileName = "TeamModels.csv";
        public const string TeamMembersFileName = "TeamMemberModels.csv";
        public const string TournamentPrizesFileName = "TournamentPrizeModels.csv";
        public const string TournamentEntriesFileName = "TournamentEntryModels.csv";
        public const string TournamentsFileName = "TournamentModels.csv";
        public const string MatchupsFileName = "MatchupModels.csv";
        public const string MatchupEntriesFileName = "MatchupEntryModels.csv";

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
