using System;
using System.Collections.Generic;
using System.Linq;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class SqlConnector : IDataConnection
    {
        private readonly ISqlDataAccess _db;
        private const string connectionStringName = "Tournaments";

        public SqlConnector(ISqlDataAccess db)
        {
            _db = db;
        }

        /// <summary>
        /// Saves a new prize to the database
        /// </summary>
        /// <param name="model">The prize information.</param>
        /// <returns>The prize information, including the unique identifier.</returns>
        public PrizeModel CreatePrize(PrizeModel model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spPrizes_Insert", new { model.PlaceNumber, model.PlaceName, model.PrizeAmount, model.PrizePercentage }, connectionStringName, true).First();
            return model;
        }

        private PrizeModel GetPrizeById(int PrizeId)
        {
            return _db.LoadDate<PrizeModel, dynamic>("dbo.spPrizes_GetById", new { PrizeId }, connectionStringName, true).First();
        }

        public PersonModel CreatePerson(PersonModel model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spPeople_Insert", new { model.FirstName, model.LastName, model.EmailAddress, model.CellphoneNumber }, connectionStringName, true).First();
            return model;
        }

        private PersonModel GetPersonById(int PersonId)
        {
            return _db.LoadDate<PersonModel, dynamic>("dbo.spPeople_GetById", new { PersonId }, connectionStringName, true).First();
        }

        public List<PersonModel> GetAllPeople()
        {
            return _db.LoadDate<PersonModel, dynamic>("dbo.spPeople_GetAll", new { }, connectionStringName, true);
        }

        public TeamModel CreateTeam(TeamModel model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spTeams_Insert", new { model.TeamName }, connectionStringName, true).First();

            foreach (PersonModel person in model.TeamMembers)
            {
                CreateTeamMember(new TeamMember { TeamId = model.Id, PersonId = person.Id });
            }

            return model;
        }

        public List<TeamModel> GetAllTeams()
        {
            var output = _db.LoadDate<TeamModel, dynamic>("dbo.spTeams_GetAll", new { }, connectionStringName, true);

            foreach (var team in output)
            {
                team.TeamMembers = _db.LoadDate<PersonModel, dynamic>("dbo.spTeamMembers_GetByTeam", new { TeamId = team.Id }, connectionStringName, true);
            }

            return output;
        }

        private TeamMember CreateTeamMember(TeamMember model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spTeamMembers_Insert", new { model.TeamId, model.PersonId }, connectionStringName, true).First();
            return model;
        }

        private List<TeamMember> GetAllTeamMembers()
        {
            return _db.LoadDate<TeamMember, dynamic>("dbo.spTeamMembers_GetAll", new { }, connectionStringName, true);
        }

        private TournamentPrizeModel CreateTournamentPrize(TournamentPrizeModel model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spTournamentPrizes_Insert", new { model.PrizeId, model.TournamentId }, connectionStringName, true).First();
            return model;
        }

        private TournamentEntryModel CreateTournamentEntry(TournamentEntryModel model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spTournamentEntries_Insert", new { model.TeamId, model.TournamentId }, connectionStringName, true).First();
            return model;
        }

        private MatchupModel CreateMatchup(MatchupModel model, int tournamentId)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spMatchups_Insert", new { model.MatchupRound, TournamentId = tournamentId }, connectionStringName, true).First();

            foreach (var entry in model.Entries)
            {
                entry.Id = _db.LoadDate<int, dynamic>("dbo.spMatchupEntries_Insert", new { MatchupId = model.Id, ParentMatchupId = entry.ParentMatchup?.Id, TeamCompetingId = entry.TeamCompeting?.Id }, connectionStringName, true).First();
            }
            
            return model;
        }

        public void CreateTournament(TournamentModel model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spTournaments_Insert", new { model.TournamentName, model.EntryFee}, connectionStringName, true).First();

            foreach (var prize in model.Prizes)
            {
                CreateTournamentPrize(new TournamentPrizeModel { PrizeId = prize.Id, TournamentId = model.Id });
            }

            foreach (var team in model.EnteredTeams)
            {
                CreateTournamentEntry(new TournamentEntryModel { TeamId = team.Id, TournamentId = model.Id });
            }

            foreach (var round in model.Rounds)
            {
                foreach (var matchup in round)
                {
                    CreateMatchup(matchup, model.Id);
                }
            }
        }
    }
}
