using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using TrackerLibrary;
using TrackerLibrary.Models;

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
            int startingRound = tournament.CheckCurrentRound();

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
            int endingRound = tournament.CheckCurrentRound();

            if (endingRound > startingRound)
            {
                tournament.AlertUsersToNewRound();
            }
        }

        public static void AlertUsersToNewRound(this TournamentModel tournament)
        {
            int currentRoundNumber = tournament.CheckCurrentRound();
            List<MatchupModel> currentRound = tournament.Rounds.Where(x => x.First().MatchupRound == currentRoundNumber).First();

            foreach (MatchupModel matchup in currentRound)
            {
                foreach (MatchupEntryModel matchupEntry in matchup.Entries)
                {
                    foreach (PersonModel person in matchupEntry.TeamCompeting.TeamMembers)
                    {
                        AlertPersonToNewRound(person, matchupEntry.TeamCompeting.TeamName, matchup.Entries.Where(x => x.TeamCompeting != matchupEntry.TeamCompeting).FirstOrDefault());
                    }
                }
            }
        }

        private static void AlertPersonToNewRound(PersonModel person, string teamName, MatchupEntryModel competitor)
        {
            if (String.IsNullOrWhiteSpace(person.EmailAddress))
            {
                return;
            }

            string to = String.Empty;
            string subject = String.Empty;
            StringBuilder body = new StringBuilder();

            if (competitor != null)
            {
                subject = $"You have a new matchup with {competitor.TeamCompeting.TeamName}.";

                body.AppendLine("<h1>You have a new matchup</h1>");
                body.Append("<string>Competitor: </strong>");
                body.Append(competitor.TeamCompeting.TeamName);
                body.AppendLine();
                body.AppendLine();
                body.AppendLine("Have a great time!");
                body.AppendLine("~Tournament Tracker");
            }
            else
            {
                subject = "You have a bye week this round.";
                body.AppendLine("Have a great time!");
                body.AppendLine("~Tournament Tracker");
            }

            to = person.EmailAddress;

            EmailLogic.SendEmail(to, subject, body.ToString());
        }

        private static int CheckCurrentRound(this TournamentModel tournament)
        {
            int output = 1;

            foreach (List<MatchupModel> round in tournament.Rounds)
            {
                if (round.All(x => x.Winner != null))
                {
                    output++;
                }
                else
                {
                    return output;
                }
            }

            CompleteTournament(tournament);

            return output - 1;
        }

        private static void CompleteTournament(TournamentModel tournament)
        {
            GlobalConfig.Connection.CompleteTournament(tournament);
            TeamModel winnerTeam = tournament.Rounds.Last().First().Winner;
            TeamModel runnerUp = tournament.Rounds.Last().First().Entries.Where(x => x.TeamCompeting != winnerTeam).First().TeamCompeting;

            decimal winnerPrize = 0;
            decimal runnerUpPrize = 0;

            if (tournament.Prizes.Count > 0)
            {
                decimal totalIncome = tournament.EnteredTeams.Count * tournament.EntryFee;

                PrizeModel firstPlacePrize = tournament.Prizes.Where(x => x.PlaceNumber == 1).FirstOrDefault();
                PrizeModel secondPlacePrize = tournament.Prizes.Where(x => x.PlaceNumber == 2).FirstOrDefault();

                if (firstPlacePrize != null)
                {
                    winnerPrize = firstPlacePrize.CalculatePrizePayout(totalIncome);
                }

                if (secondPlacePrize != null)
                {
                    runnerUpPrize = secondPlacePrize.CalculatePrizePayout(totalIncome);
                }
            }

            string subject = "";
            StringBuilder body = new StringBuilder();

            subject = $"in {tournament.TournamentName}, {winnerTeam.TeamName} has won!";

            body.AppendLine("<h1>You have a WINNER!</h1>");
            body.AppendLine("<p>Congratulations to our winner on a great tournament.</p>");
            body.AppendLine("<br/>");
            
            if (winnerPrize > 0)
            {
                body.AppendLine($"<p>{winnerTeam.TeamName} will receive ${winnerPrize}</p>");
            }

            if (runnerUpPrize > 0)
            {
                body.AppendLine($"<p>{runnerUp.TeamName} will receive ${runnerUpPrize}</p>");
            }

            body.AppendLine("<p>Thanks for a great tournament everyone!</p>");
            body.AppendLine("~Tournament Tracker");

            List<string> bcc = new List<string>();

            foreach (TeamModel team in tournament.EnteredTeams)
            {
                foreach (PersonModel person in team.TeamMembers)
                {
                    if (!String.IsNullOrWhiteSpace(person.EmailAddress))
                    {
                        bcc.Add(person.EmailAddress);
                    }
                }
            }

            EmailLogic.SendEmail(new List<string>(), bcc, subject, body.ToString());

            tournament.CompleteTournament();
        }

        private static decimal CalculatePrizePayout(this PrizeModel prize, decimal totalIncome)
        {
            decimal output = 0;

            if (prize.PrizeAmount > 0)
            {
                output = prize.PrizeAmount;
            }
            else
            {
                output = Decimal.Multiply(totalIncome, Convert.ToDecimal(prize.PrizePercentage / 100));
            }

            return output;
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
                        throw new ArgumentException("Invalid value in the File (App.config) key=greaterWins");
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
            else
            {
                throw new Exception("We do not allow ties in this application.");
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
            else
            {
                throw new Exception("We do not allow ties in this application.");
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
