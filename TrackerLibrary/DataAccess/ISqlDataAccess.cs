using System.Collections.Generic;

namespace TrackerLibrary.DataAccess
{
    public interface ISqlDataAccess
    {
        List<T> LoadDate<T, U>(string sqlStatement, U parameters, string connectionStringName, bool isStoredProcedure = false);
        void SaveData<T>(string sqlStatement, T parameters, string connectionStringName, bool isStoredProcedure = false);
    }
}