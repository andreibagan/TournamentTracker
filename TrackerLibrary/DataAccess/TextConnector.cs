using System.Collections.Generic;
using System.Linq;
using TournamentTracker.DataAccess;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        private readonly ITextFileDataAccess _db;
        private const string PrizesFileName = "PrizesModels.csv";

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
    }
}
