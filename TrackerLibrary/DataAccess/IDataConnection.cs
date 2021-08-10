using System.Collections.Generic;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public interface IDataConnection
    {
        void CreatePrize(PrizeModel model);
        PrizeModel GetPrizeById(int PrizeId);

        void CreatePerson(PersonModel model);
        PersonModel GetPersonById(int PersonId);
        List<PersonModel> GetAllPeople();

        void CreateTeam(TeamModel model);
        List<TeamModel> GetAllTeams();

        void CreateTeamMember(TeamMember model);
        List<TeamMember> GetAllTeamMembers();
    }
}
