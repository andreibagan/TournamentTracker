using System;
using System.Collections.Generic;
using System.Linq;
using TournamentTracker;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        private readonly ITextFileDataAccess _db;

        public TextConnector(ITextFileDataAccess db)
        {
            _db = db;
        }

        public void CreatePrize(PrizeModel model)
        {
            List<PrizeModel> prizes = _db.LoadFromTextFile<PrizeModel>(GlobalConfig.PrizesFileName);

            int currentId = 1;

            if (prizes.Count > 0)
            {
                currentId = prizes.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            prizes.Add(model);

            _db.SaveToTextFile(prizes, GlobalConfig.PrizesFileName);
        }

        private PrizeModel GetPrizeById(int PrizeId)
        {
            PrizeModel prize;
            List<PrizeModel> prizes = _db.LoadFromTextFile<PrizeModel>(GlobalConfig.PrizesFileName);

            prize = prizes.Find(p => p.Id == PrizeId);

            return prize;
        }

        public List<PrizeModel> GetAllPrizes()
        {
            return _db.LoadFromTextFile<PrizeModel>(GlobalConfig.PrizesFileName);
        }

        public void CreatePerson(PersonModel model)
        {
            List<PersonModel> people = GetAllPeople();

            int currentId = 1;

            if (people.Count > 0)
            {
                currentId = people.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            people.Add(model);

            _db.SaveToTextFile(people, GlobalConfig.PeopleFileName);
        }

        private PersonModel GetPersonById(int PersonId)
        {
            PersonModel person;
            List<PersonModel> people = GetAllPeople();

            person = people.Find(p => p.Id == PersonId);

            return person;
        }

        public List<PersonModel> GetAllPeople()
        {
            return _db.LoadFromTextFile<PersonModel>(GlobalConfig.PeopleFileName);
        }

        public void CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = _db.LoadFromTextFile<TeamModel>(GlobalConfig.TeamsFileName);

            int currentId = 1;

            if (teams.Count > 0)
            {
                currentId = teams.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            teams.Add(model);

            _db.SaveToTextFile(teams, GlobalConfig.TeamsFileName);

            foreach (var person in model.TeamMembers)
            {
                CreateTeamMember(new TeamMember { TeamId = model.Id, PersonId = person.Id });
            }
        }

        public List<TeamModel> GetAllTeams()
        {
            var output = _db.LoadFromTextFile<TeamModel>(GlobalConfig.TeamsFileName);
            var teamMembers = GetAllTeamMembers();
            var people = GetAllPeople();

            foreach (var team in output)
            {
                team.TeamMembers = people.Where(p => teamMembers.Where(t => t.TeamId == team.Id).Select(t => t.PersonId).Contains(p.Id)).ToList();
            }

            return output;
        }

        private List<TeamModel> GetAllTeamsLite()
        {
            return _db.LoadFromTextFile<TeamModel>(GlobalConfig.TeamsFileName);
        }

        private void CreateTeamMember(TeamMember model)
        {
            List<TeamMember> teamMembers = GetAllTeamMembers();

            int currentId = 1;

            if (teamMembers.Count > 0)
            {
                currentId = teamMembers.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            teamMembers.Add(model);

            _db.SaveToTextFile(teamMembers, GlobalConfig.TeamMembersFileName);
        }

        private List<TeamMember> GetAllTeamMembers()
        {
            return _db.LoadFromTextFile<TeamMember>(GlobalConfig.TeamMembersFileName);
        }

        private void CreateTournamentPrize(TournamentPrizeModel model)
        {
            List<TournamentPrizeModel> tournamentPrizes = GetAllTournamentPrizes();

            int currentId = 1;

            if (tournamentPrizes.Count > 0)
            {
                currentId = tournamentPrizes.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            tournamentPrizes.Add(model);

            _db.SaveToTextFile(tournamentPrizes, GlobalConfig.TournamentPrizesFileName);
        }

        private List<TournamentPrizeModel> GetAllTournamentPrizes()
        {
            return _db.LoadFromTextFile<TournamentPrizeModel>(GlobalConfig.TournamentPrizesFileName);
        }

        private void CreateTournamentEntry(TournamentEntryModel model)
        {
            List<TournamentEntryModel> tournamentEntries = GetAllTournamentEntries();

            int currentId = 1;

            if (tournamentEntries.Count > 0)
            {
                currentId = tournamentEntries.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            tournamentEntries.Add(model);

            _db.SaveToTextFile(tournamentEntries, GlobalConfig.TournamentEntriesFileName);
        }

        private List<TournamentEntryModel> GetAllTournamentEntries()
        {
            return _db.LoadFromTextFile<TournamentEntryModel>(GlobalConfig.TournamentEntriesFileName);
        }

        private void CreateMatchupEntry(MatchupEntryModel model, int matchupId)
        {
            List<MatchupEntryModel> matchupEntries = GetAllMatchupEntries();

            int currentId = 1;

            if (matchupEntries.Count > 0)
            {
                currentId = matchupEntries.Max(p => p.Id) + 1;
            }

            model.Id = currentId;
            model.MatchupId = matchupId;
            model.ParentMatchupId = model.ParentMatchup != null ? model.ParentMatchup.Id : 0;

            matchupEntries.Add(model);

            _db.SaveToTextFile(matchupEntries, GlobalConfig.MatchupEntriesFileName);
        }

        private List<MatchupEntryModel> GetAllMatchupEntries()
        {
            return _db.LoadFromTextFile<MatchupEntryModel>(GlobalConfig.MatchupEntriesFileName);
        }

        private void CreateMatchup(MatchupModel model, int tournamentId)
        {
            List<MatchupModel> matchups = GetAllMatchups();

            int currentId = 1;

            if (matchups.Count > 0)
            {
                currentId = matchups.Max(p => p.Id) + 1;
            }

            model.Id = currentId;
            model.TournamentId = tournamentId;

            matchups.Add(model);

            _db.SaveToTextFile(matchups, GlobalConfig.MatchupsFileName);

            foreach (var entry in model.Entries)
            {
                CreateMatchupEntry(entry, model.Id);
            }
        }

        private List<MatchupModel> GetAllMatchups()
        {
            return _db.LoadFromTextFile<MatchupModel>(GlobalConfig.MatchupsFileName);
        }

        public void UpdateMatchup(MatchupModel model)
        {
            List<MatchupModel> matchups = GetAllMatchups();
            List<MatchupEntryModel> matchupEntries = GetAllMatchupEntries();

            MatchupModel matchup = matchups.Where(m => m.Id == model.Id).First();
            matchups.Remove(matchup);
            matchups.Add(model);

            _db.SaveToTextFile(matchups.OrderBy(m => m.Id).ToList(), GlobalConfig.MatchupsFileName);

            foreach (MatchupEntryModel entry in model.Entries)
            {
                MatchupEntryModel matchupEntry = matchupEntries.Where(m => m.Id == entry.Id).First();
                matchupEntries.Remove(matchupEntry);
                matchupEntries.Add(entry);
            }

            _db.SaveToTextFile(matchupEntries.OrderBy(m => m.Id).ToList(), GlobalConfig.MatchupEntriesFileName);
        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GetAllTournamentsLite();

            int currentId = 1;

            if (tournaments.Count > 0)
            {
                currentId = tournaments.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            tournaments.Add(model);

            _db.SaveToTextFile(tournaments, GlobalConfig.TournamentsFileName);

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
                foreach (var match in round)
                {
                    CreateMatchup(match, model.Id);
                }
            }

            TournamentLogic.UpdateTournamentResults(model);
        }

        private List<TournamentModel> GetAllTournamentsLite()
        {
            return _db.LoadFromTextFile<TournamentModel>(GlobalConfig.TournamentsFileName);
        }

        public List<TournamentModel> GetAllTournaments()
        {
            List<TournamentModel> output;

            output = GetAllTournamentsLite();

            foreach (TournamentModel tournament in output)
            {
                tournament.Prizes = GetAllPrizesByTournamentId(tournament.Id);
                tournament.EnteredTeams = GetAllTeamsByTournamentId(tournament.Id);

                List<MatchupModel> matchups = GetAllMatchupsByTournamentId(tournament.Id);

                foreach (var matchup in matchups)
                {
                    matchup.Entries = GetAllMatchupEntriesByMatchupId(matchup.Id);

                    List<TeamModel> teams = GetAllTeamsLite();

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

        private List<MatchupEntryModel> GetAllMatchupEntriesByMatchupId(int matchupId)
        {
            List<MatchupEntryModel> output = new List<MatchupEntryModel>();
            List<MatchupEntryModel> matchupEntries = GetAllMatchupEntries();

            output = matchupEntries.Where(me => me.MatchupId == matchupId).ToList();

            return output;
        }

        private List<MatchupModel> GetAllMatchupsByTournamentId(int tournamentId)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            List<MatchupModel> matchups = GetAllMatchups();

            output = matchups.Where(m => m.TournamentId == tournamentId).ToList();

            return output;
        }

        private List<TeamModel> GetAllTeamsByTournamentId(int tournamentId)
        {
            List<TeamModel> output = new List<TeamModel>();

            List<TournamentEntryModel> allTournamentEntries = GetAllTournamentEntries();
            List<TeamModel> teams = GetAllTeams();

            List<TournamentEntryModel> tournamentEntries = allTournamentEntries.Where(t => t.TournamentId == tournamentId).ToList();
            output = teams.Join(tournamentEntries, t => t.Id, te => te.TeamId, (t, te) => t).ToList();

            return output;
        }

        private List<PrizeModel> GetAllPrizesByTournamentId(int tournamentId)
        {
            List<PrizeModel> output = new List<PrizeModel>();

            List<TournamentPrizeModel> allTournamentPrizes = GetAllTournamentPrizes();
            List<PrizeModel> prizes = GetAllPrizes();

            List<TournamentPrizeModel> tournamentPrizes = allTournamentPrizes.Where(tp => tp.TournamentId == tournamentId).ToList();
            output = prizes.Join(tournamentPrizes, p => p.Id, tp => tp.PrizeId, (p, tp) => p).ToList();

            return output;
        }

        public void CompleteTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GetAllTournamentsLite();

            tournaments.Remove(model);

            _db.SaveToTextFile(tournaments, GlobalConfig.TournamentsFileName);

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
                foreach (var match in round)
                {
                    CreateMatchup(match, model.Id);
                }
            }

            TournamentLogic.UpdateTournamentResults(model);
        }
    }
}
