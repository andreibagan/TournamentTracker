using System.Collections.Generic;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public interface IDataConnection
    {
        void CreatePrize(PrizeModel model);

        void CreatePerson(PersonModel model);
        List<PersonModel> GetAllPeople();

        void CreateTeam(TeamModel model);
        List<TeamModel> GetAllTeams();

        void CreateTournament(TournamentModel model);
        List<TournamentModel> GetAllTournaments();

        void UpdateMatchup(MatchupModel model);
    }
}
