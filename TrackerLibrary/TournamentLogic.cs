using System.Collections.Generic;
using TrackerLibrary.Models;
using System.Linq;
using System;
using System.Configuration;
using TrackerLibrary;

namespace TournamentTracker
{
    public static class TournamentLogic
    {
        public static void CreateRounds(TournamentModel tournament)
        {
            List<TeamModel> randomizedTeams = RandomizeTeamOrder(tournament.EnteredTeams);
            int rounds = FindNumberOfRounds(randomizedTeams.Count);
            int byes = NumberOfByes(rounds, randomizedTeams.Count);

            tournament.Rounds.Add(CreateFirstRound(byes, randomizedTeams));

            CreateOtherRounds(tournament, rounds);

            UpdateTournamentResults(tournament);
        }

        public static void UpdateTournamentResults(TournamentModel tournament)
        {
            List<MatchupModel> toScore = new List<MatchupModel>();

            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                foreach (MatchupModel rm in round)
                {
                    if (rm.Winner == null && (rm.Entries.Any(x => x.Score != 0) || rm.Entries.Count == 1))
                    {
                        toScore.Add(rm);
                    }
                }
            }

            MarkWinnerInMatchups(toScore);

            AdvanceWinners(toScore, tournament);

            toScore.ForEach(x => GlobalConfig.Connection.UpdateMatchup(x));
        }

        private static void AdvanceWinners(List<MatchupModel> toScore, TournamentModel tournament)
        {
            foreach (MatchupModel matchup in toScore)
            {
                foreach (List<MatchupModel> round in tournament.Rounds)
                {
                    foreach (MatchupModel rm in round)
                    {
                        foreach (MatchupEntryModel me in rm.Entries)
                        {
                            if (me.ParentMatchup?.Id == matchup.Id)
                            {
                                me.TeamCompeting = matchup.Winner;
                                GlobalConfig.Connection.UpdateMatchup(rm);
                            }
                        }
                    }
                }
            }
        }

        private static void MarkWinnerInMatchups(List<MatchupModel> matchups)
        {
            string greaterWins = ConfigurationManager.AppSettings["greaterWins"];

            foreach (MatchupModel matchup in matchups)
            {
                if (matchup.Entries.Count == 1)
                {
                    matchup.Winner = matchup.Entries[0].TeamCompeting;
                    continue;
                }

                switch (greaterWins)
                {
                    case "0":
                        LesserScoreWin(matchup);
                        break;
                    case "1":
                        GreaterScoreWin(matchup);
                        break;
                    default:
                        throw new Exception("We do not allow ties in this application.");
                }
            }
        }

        private static void GreaterScoreWin(MatchupModel matchup)
        {
            if (matchup.Entries[0].Score > matchup.Entries[1].Score)
            {
                matchup.Winner = matchup.Entries[0].TeamCompeting;
            }
            else if (matchup.Entries[0].Score < matchup.Entries[1].Score)
            {
                matchup.Winner = matchup.Entries[1].TeamCompeting;
            }
        }

        private static void LesserScoreWin(MatchupModel matchup)
        {
            if (matchup.Entries[0].Score < matchup.Entries[1].Score)
            {
                matchup.Winner = matchup.Entries[0].TeamCompeting;
            }
            else if (matchup.Entries[0].Score > matchup.Entries[1].Score)
            {
                matchup.Winner = matchup.Entries[1].TeamCompeting;
            }
        }

        private static void CreateOtherRounds(TournamentModel tournament, int rounds)
        {
            int round = 2;
            List<MatchupModel> previousRound = tournament.Rounds.First();
            List<MatchupModel> currRound = new List<MatchupModel>();
            MatchupModel currMatchup = new MatchupModel();

            while (round <= rounds)
            {
                foreach (MatchupModel match in previousRound)
                {
                    currMatchup.Entries.Add(new MatchupEntryModel { ParentMatchup = match });

                    if (currMatchup.Entries.Count > 1)
                    {
                        currMatchup.MatchupRound = round;
                        currRound.Add(currMatchup);
                        currMatchup = new MatchupModel();
                    }
                }

                tournament.Rounds.Add(currRound);
                previousRound = currRound;

                currRound = new List<MatchupModel>();
                round++;
            }
        }

        private static List<MatchupModel> CreateFirstRound(int byes, List<TeamModel> teams)
        {
            List<MatchupModel> output = new List<MatchupModel>();
            MatchupModel curr = new MatchupModel();

            foreach (TeamModel team in teams)
            {
                curr.Entries.Add(new MatchupEntryModel { TeamCompetingId = team.Id, TeamCompeting = team });

                if (byes > 0 || curr.Entries.Count > 1)
                {
                    curr.MatchupRound = 1;
                    output.Add(curr);
                    curr = new MatchupModel();

                    if (byes > 0)
                    {
                        byes--;
                    }
                }
            }

            return output;
        }

        private static int NumberOfByes(int rounds, int numberOfTeams)
        {
            return (int)Math.Pow(2, rounds) - numberOfTeams;
        }

        private static int FindNumberOfRounds(int teamCount)
        {
            int output = 1;
            int val = 2;

            while (val < teamCount)
            {
                output++;
                val *= 2;
            }

            return output;
        }

        private static List<TeamModel> RandomizeTeamOrder(List<TeamModel> teams)
        {
            return teams.OrderBy(t => Guid.NewGuid()).ToList();
        }
    }
}
