using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public interface IDataConnection
    {
        void CreatePrize(PrizeModel model);
        PrizeModel GetPrizeById(int PrizeId);
        void CreatePerson(PersonModel model);
        PersonModel GetPersonById(int PersonId);
    }
}
