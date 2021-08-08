using System.Collections.Generic;
using System.Linq;
using TournamentTracker.DataAccess;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        private readonly ITextFileDataAccess _db;
        private const string PrizesFileName = "PrizeModels.csv";
        private const string PeopleFileName = "PersonModels.csv";

        public TextConnector(ITextFileDataAccess db)
        {
            _db = db;
        }

        public void CreatePrize(PrizeModel model)
        {
            List<PrizeModel> prizes = _db.LoadFromTextFile<PrizeModel>(PrizesFileName);

            int currentId = 1;

            if (prizes.Count > 0)
            {
                currentId = prizes.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            prizes.Add(model);

            _db.SaveToTextFile(prizes, PrizesFileName);
        }

        public PrizeModel GetPrizeById(int PrizeId)
        {
            PrizeModel prize;
            List<PrizeModel> prizes = _db.LoadFromTextFile<PrizeModel>(PrizesFileName);

            prize = prizes.Find(p => p.Id == PrizeId);

            return prize;
        }

        public void CreatePerson(PersonModel model)
        {
            List<PersonModel> people = _db.LoadFromTextFile<PersonModel>(PeopleFileName);

            int currentId = 1;

            if (people.Count > 0)
            {
                currentId = people.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            people.Add(model);

            _db.SaveToTextFile(people, PeopleFileName);
        }

        public PersonModel GetPersonById(int PersonId)
        {
            PersonModel person;
            List<PersonModel> people = _db.LoadFromTextFile<PersonModel>(PeopleFileName);

            person = people.Find(p => p.Id == PersonId);

            return person;
        }
    }
}
