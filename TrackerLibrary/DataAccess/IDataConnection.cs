using System.Collections.Generic;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public interface IDataConnection
    {
        void CreatePrize(PrizeModel model);
        List<PrizeModel> GetAllPrizes();

        void CreatePerson(PersonModel model);
        List<PersonModel> GetAllPeople();

        void CreateTeam(TeamModel model);
        List<TeamModel> GetAllTeams();

        void CreateTournament(TournamentModel model);
        List<TournamentModel> GetAllTournaments();
        void CompleteTournament(TournamentModel model);

        void UpdateMatchup(MatchupModel model);
    }
}
