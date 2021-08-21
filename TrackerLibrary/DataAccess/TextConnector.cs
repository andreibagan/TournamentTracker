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

        public PrizeModel GetPrizeById(int PrizeId)
        {
            PrizeModel prize;
            List<PrizeModel> prizes = _db.LoadFromTextFile<PrizeModel>(PrizesFileName);

            prize = prizes.Find(p => p.Id == PrizeId);

            return prize;
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

        public PersonModel GetPersonById(int PersonId)
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

        public TeamMember CreateTeamMember(TeamMember model)
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

        public List<TeamMember> GetAllTeamMembers()
        {
            return _db.LoadFromTextFile<TeamMember>(TeamMembersFileName);
        }

        public TournamentPrizeModel CreateTournamentPrize(TournamentPrizeModel model)
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

        public List<TournamentPrizeModel> GetAllTournamentPrizes()
        {
            return _db.LoadFromTextFile<TournamentPrizeModel>(TournamentPrizesFileName);
        }

        public TournamentEntryModel CreateTournamentEntry(TournamentEntryModel model)
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

        public List<TournamentEntryModel> GetAllTournamentEntries()
        {
            return _db.LoadFromTextFile<TournamentEntryModel>(TournamentEntriesFileName);
        }

        private MatchupEntryModel CreateMatchupEntry(MatchupEntryModel model)
        {
            List<MatchupEntryModel> matchupEntries = GetAllMatchupEntries();

            int currentId = 1;

            if (matchupEntries.Count > 0)
            {
                currentId = matchupEntries.Max(p => p.Id) + 1;
            }

            model.Id = currentId;
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

            matchups.Add(model);

            _db.SaveToTextFile(matchups, MatchupsFileName);

            foreach (var entry in model.Entries)
            {
                CreateMatchupEntry(entry);
            }

            return model;
        }

        private List<MatchupModel> GetAllMatchups()
        {
            return _db.LoadFromTextFile<MatchupModel>(MatchupsFileName);
        }

        public void CreateTournament(TournamentModel model)
        {
            List<TournamentModel> tournaments = GetAllTournaments();

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

        public List<TournamentModel> GetAllTournaments()
        {
            return _db.LoadFromTextFile<TournamentModel>(TournamentsFileName);
        }
    }
}
