using System.Collections.Generic;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public interface IDataConnection
    {
        PrizeModel CreatePrize(PrizeModel model);

        PersonModel CreatePerson(PersonModel model);

        List<PersonModel> GetAllPeople();

        TeamModel CreateTeam(TeamModel model);
        List<TeamModel> GetAllTeams();

        void CreateTournament(TournamentModel model);
        List<TournamentModel> GetAllTournaments();

        void UpdateMatchup(MatchupModel model);
    }
}
