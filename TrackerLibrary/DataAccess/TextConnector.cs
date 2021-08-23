using System;
using System.Collections.Generic;
using System.Linq;
using TrackerLibrary.Models;

namespace TrackerLibrary.DataAccess
{
    public class TextConnector : IDataConnection
    {
        private readonly ITextFileDataAccess _db;
        private const string PrizesFileName = "PrizeModels.csv";
        private const string PeopleFileName = "PersonModels.csv";
        private const string TeamsFileName = "TeamModels.csv";
        private const string TeamMembersFileName = "TeamMemberModels.csv";
        private const string TournamentPrizesFileName = "TournamentPrizeModels.csv";
        private const string TournamentEntriesFileName = "TournamentEntryModels.csv";
        private const string TournamentsFileName = "TournamentModels.csv";
        private const string MatchupsFileName = "MatchupModels.csv";
        private const string MatchupEntriesFileName = "MatchupEntryModels.csv";

        public TextConnector(ITextFileDataAccess db)
        {
            _db = db;
        }

        public PrizeModel CreatePrize(PrizeModel model)
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

            return model;
        }

        private PrizeModel GetPrizeById(int PrizeId)
        {
            PrizeModel prize;
            List<PrizeModel> prizes = _db.LoadFromTextFile<PrizeModel>(PrizesFileName);

            prize = prizes.Find(p => p.Id == PrizeId);

            return prize;
        }

        private List<PrizeModel> GetAllPrizes()
        {
            return _db.LoadFromTextFile<PrizeModel>(PrizesFileName);
        }

        public PersonModel CreatePerson(PersonModel model)
        {
            List<PersonModel> people = GetAllPeople();

            int currentId = 1;

            if (people.Count > 0)
            {
                currentId = people.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            people.Add(model);

            _db.SaveToTextFile(people, PeopleFileName);

            return model;
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
            return _db.LoadFromTextFile<PersonModel>(PeopleFileName);
        }

        public TeamModel CreateTeam(TeamModel model)
        {
            List<TeamModel> teams = _db.LoadFromTextFile<TeamModel>(TeamsFileName);

            int currentId = 1;

            if (teams.Count > 0)
            {
                currentId = teams.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            teams.Add(model);

            _db.SaveToTextFile(teams, TeamsFileName);

            foreach (var person in model.TeamMembers)
            {
                CreateTeamMember(new TeamMember { TeamId = model.Id, PersonId = person.Id });
            }

            return model;
        }

        public List<TeamModel> GetAllTeams()
        {
            var output = _db.LoadFromTextFile<TeamModel>(TeamsFileName);
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
            return _db.LoadFromTextFile<TeamModel>(TeamsFileName);
        }

        private TeamMember CreateTeamMember(TeamMember model)
        {
            List<TeamMember> teamMembers = GetAllTeamMembers();

            int currentId = 1;

            if (teamMembers.Count > 0)
            {
                currentId = teamMembers.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            teamMembers.Add(model);

            _db.SaveToTextFile(teamMembers, TeamMembersFileName);

            return model;
        }

        private List<TeamMember> GetAllTeamMembers()
        {
            return _db.LoadFromTextFile<TeamMember>(TeamMembersFileName);
        }

        private TournamentPrizeModel CreateTournamentPrize(TournamentPrizeModel model)
        {
            List<TournamentPrizeModel> tournamentPrizes = GetAllTournamentPrizes();

            int currentId = 1;

            if (tournamentPrizes.Count > 0)
            {
                currentId = tournamentPrizes.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            tournamentPrizes.Add(model);

            _db.SaveToTextFile(tournamentPrizes, TournamentPrizesFileName);

            return model;
        }

        private List<TournamentPrizeModel> GetAllTournamentPrizes()
        {
            return _db.LoadFromTextFile<TournamentPrizeModel>(TournamentPrizesFileName);
        }

        private TournamentEntryModel CreateTournamentEntry(TournamentEntryModel model)
        {
            List<TournamentEntryModel> tournamentEntries = GetAllTournamentEntries();

            int currentId = 1;

            if (tournamentEntries.Count > 0)
            {
                currentId = tournamentEntries.Max(p => p.Id) + 1;
            }

            model.Id = currentId;

            tournamentEntries.Add(model);

            _db.SaveToTextFile(tournamentEntries, TournamentEntriesFileName);

            return model;
        }

        private List<TournamentEntryModel> GetAllTournamentEntries()
        {
            return _db.LoadFromTextFile<TournamentEntryModel>(TournamentEntriesFileName);
        }

        private MatchupEntryModel CreateMatchupEntry(MatchupEntryModel model, int matchupId)
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

            _db.SaveToTextFile(matchupEntries, MatchupEntriesFileName);

            return model;
        }

        private List<MatchupEntryModel> GetAllMatchupEntries()
        {
            return _db.LoadFromTextFile<MatchupEntryModel>(MatchupEntriesFileName);
        }

        private MatchupModel CreateMatchup(MatchupModel model, int tournamentId)
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

            _db.SaveToTextFile(matchups, MatchupsFileName);

            foreach (var entry in model.Entries)
            {
                CreateMatchupEntry(entry, model.Id);
            }

            return model;
        }

        private List<MatchupModel> GetAllMatchups()
        {
            return _db.LoadFromTextFile<MatchupModel>(MatchupsFileName);
        }

        public void UpdateMatchup(MatchupModel model)
        {
            List<MatchupModel> matchups = GetAllMatchups();
            List<MatchupEntryModel> matchupEntries = GetAllMatchupEntries();

            MatchupModel matchup = matchups.Where(m => m.Id == model.Id).First();
            matchups.Remove(matchup);
            matchups.Add(model);

            _db.SaveToTextFile(matchups.OrderBy(m => m.Id).ToList(), MatchupsFileName);

            foreach (MatchupEntryModel entry in model.Entries)
            {
                MatchupEntryModel matchupEntry = matchupEntries.Where(m => m.Id == entry.Id).First();
                matchupEntries.Remove(matchupEntry);
                matchupEntries.Add(entry);
            }

            _db.SaveToTextFile(matchupEntries.OrderBy(m => m.Id).ToList(), MatchupEntriesFileName);
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

            _db.SaveToTextFile(tournaments, TournamentsFileName);

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
        }

        private List<TournamentModel> GetAllTournamentsLite()
        {
            return _db.LoadFromTextFile<TournamentModel>(TournamentsFileName);
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
    }
}
