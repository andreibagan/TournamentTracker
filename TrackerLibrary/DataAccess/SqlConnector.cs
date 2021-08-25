using System;
using System.Collections.Generic;
using System.Linq;
using TournamentTracker;
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
        public void CreatePrize(PrizeModel model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spPrizes_Insert", new { model.PlaceNumber, model.PlaceName, model.PrizeAmount, model.PrizePercentage }, connectionStringName, true).First();
        }

        private PrizeModel GetPrizeById(int PrizeId)
        {
            return _db.LoadDate<PrizeModel, dynamic>("dbo.spPrizes_GetById", new { PrizeId }, connectionStringName, true).First();
        }

        public void CreatePerson(PersonModel model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spPeople_Insert", new { model.FirstName, model.LastName, model.EmailAddress, model.CellphoneNumber }, connectionStringName, true).First();
        }

        private PersonModel GetPersonById(int PersonId)
        {
            return _db.LoadDate<PersonModel, dynamic>("dbo.spPeople_GetById", new { PersonId }, connectionStringName, true).First();
        }

        public List<PersonModel> GetAllPeople()
        {
            return _db.LoadDate<PersonModel, dynamic>("dbo.spPeople_GetAll", new { }, connectionStringName, true);
        }

        public void CreateTeam(TeamModel model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spTeams_Insert", new { model.TeamName }, connectionStringName, true).First();

            foreach (PersonModel person in model.TeamMembers)
            {
                CreateTeamMember(new TeamMember { TeamId = model.Id, PersonId = person.Id });
            }
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

        private void CreateTeamMember(TeamMember model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spTeamMembers_Insert", new { model.TeamId, model.PersonId }, connectionStringName, true).First();
        }

        private List<TeamMember> GetAllTeamMembers()
        {
            return _db.LoadDate<TeamMember, dynamic>("dbo.spTeamMembers_GetAll", new { }, connectionStringName, true);
        }

        private void CreateTournamentPrize(TournamentPrizeModel model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spTournamentPrizes_Insert", new { model.PrizeId, model.TournamentId }, connectionStringName, true).First();
        }

        private void CreateTournamentEntry(TournamentEntryModel model)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spTournamentEntries_Insert", new { model.TeamId, model.TournamentId }, connectionStringName, true).First();
        }

        private void CreateMatchup(MatchupModel model, int tournamentId)
        {
            model.Id = _db.LoadDate<int, dynamic>("dbo.spMatchups_Insert", new { model.MatchupRound, TournamentId = tournamentId }, connectionStringName, true).First();

            foreach (var entry in model.Entries)
            {
                entry.Id = _db.LoadDate<int, dynamic>("dbo.spMatchupEntries_Insert", new { MatchupId = model.Id, ParentMatchupId = entry.ParentMatchup?.Id, TeamCompetingId = entry.TeamCompeting?.Id }, connectionStringName, true).First();
            }
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

            TournamentLogic.UpdateTournamentResults(model);
        }

        public List<TournamentModel> GetAllTournaments()
        {
            List<TournamentModel> output;

            output = _db.LoadDate<TournamentModel, dynamic>("dbo.spTournaments_GetAll", new { }, connectionStringName, true);

            foreach (TournamentModel tournament in output)
            {
                tournament.Prizes = _db.LoadDate<PrizeModel, dynamic>("dbo.spPrizes_GetByTournament", new { TournamentId = tournament.Id }, connectionStringName, true);
                tournament.EnteredTeams = _db.LoadDate<TeamModel, dynamic>("dbo.spTeams_GetByTournament", new { TournamentId = tournament.Id }, connectionStringName, true);

                foreach (var team in tournament.EnteredTeams)
                {
                    team.TeamMembers = _db.LoadDate<PersonModel, dynamic>("dbo.spTeamMembers_GetByTeam", new { TeamId = team.Id }, connectionStringName, true);
                }

                List<MatchupModel> matchups = _db.LoadDate<MatchupModel, dynamic>("dbo.spMatchups_GetByTournament", new { TournamentId = tournament.Id }, connectionStringName, true);

                foreach (var matchup in matchups)
                {
                    matchup.Entries = _db.LoadDate<MatchupEntryModel, dynamic>("dbo.spMatchupEntries_GetByMatchup", new { MatchupId = matchup.Id }, connectionStringName, true);

                    List<TeamModel> teams = GetAllTeams();

                    if (matchup.WinnerId > 0)
                    {
                        matchup.Winner = teams.Where(x => x.Id == matchup.WinnerId).First();
                    }

                    foreach (var entry in matchup.Entries)
                    {
                        if (entry.TeamCompetingId > 0)
                        {
                            entry.TeamCompeting = teams.Where(x => x.Id == entry.TeamCompetingId).First();
                        }

                        if (entry.ParentMatchupId > 0)
                        {
                            entry.ParentMatchup = matchups.Where(x => x.Id == entry.ParentMatchupId).First();
                        }
                    }
                }

                List<MatchupModel> currRow = new List<MatchupModel>();
                int currRound = 1;

                foreach (MatchupModel matchupModel in matchups)
                {
                    if (matchupModel.MatchupRound > currRound)
                    {
                        tournament.Rounds.Add(currRow);
                        currRow = new List<MatchupModel>();
                        currRound++;
                    }

                    currRow.Add(matchupModel);
                }

                tournament.Rounds.Add(currRow);
            }

            return output;
        }

        public void UpdateMatchup(MatchupModel model)
        {
            if (model.Winner != null)
            {
                _db.SaveData("dbo.spMatchups_Update", new { id = model.Id, WinnerId = model.Winner.Id }, connectionStringName, true);
            }
           
            foreach (MatchupEntryModel entry in model.Entries)
            {
                if (entry.TeamCompeting != null)
                {
                    _db.SaveData("dbo.spMatchupEntries_Update", new { id = entry.Id, TeamCompetingId = entry.TeamCompeting.Id, Score = entry.Score }, connectionStringName, true); 
                }
            }
        }   
    }
}
