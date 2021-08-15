using System.Collections.Generic;

namespace TrackerLibrary.DataAccess
{
    public interface ITextFileDataAccess
    {
        List<T> LoadFromTextFile<T>(string fileName) where T : new();
        void SaveToTextFile<T>(List<T> data, string fileName);
    }
}