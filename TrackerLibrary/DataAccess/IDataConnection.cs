using System.Collections.Generic;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public interface IDataConnection
    {
        PrizeModel CreatePrize(PrizeModel model);
        //PrizeModel GetPrizeById(int PrizeId);

        PersonModel CreatePerson(PersonModel model);
        //PersonModel GetPersonById(int PersonId);
        List<PersonModel> GetAllPeople();

        TeamModel CreateTeam(TeamModel model);
        List<TeamModel> GetAllTeams();

        //TeamMember CreateTeamMember(TeamMember model);
        //List<TeamMember> GetAllTeamMembers();

        //TournamentPrizeModel CreateTournamentPrize(TournamentPrizeModel model);

        //TournamentEntryModel CreateTournamentEntry(TournamentEntryModel model);

        void CreateTournament(TournamentModel model);
        List<TournamentModel> GetAllTournaments();
    }
}
