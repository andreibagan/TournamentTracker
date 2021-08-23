using System.Collections.Generic;
using System.Text;
using TournamentTracker.Attributes;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one match int the tournament.
    /// </summary>
    public class MatchupModel
    {
        /// <summary>
        /// The unique identifier for the matchup.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The unique identifier for the tournament.
        /// </summary>
        public int TournamentId { get; set; }

        /// <summary>
        /// Which round this match is a part of.
        /// </summary>
        public int MatchupRound { get; set; }

        /// <summary>
        /// The ID from the database that will be used to identify the winner.
        /// </summary>
        public int WinnerId { get; set; }

        /// <summary>
        /// The winner of the match.
        /// </summary>
        [RemoveProperty]
        public TeamModel Winner { get; set; }

        /// <summary>
        /// The set of teams that were involved in this match.
        /// </summary>
        [RemoveProperty]
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();

        [RemoveProperty]
        public string DisplayName
        {
            get
            {
                StringBuilder output = new StringBuilder();

                foreach (MatchupEntryModel entry in Entries)
                {
                    if (entry.TeamCompeting != null)
                    {
                        if (output.Length == 0)
                        {
                            output.Append(entry.TeamCompeting.TeamName);
                        }
                        else
                        {
                            output.Append($" vs. {entry.TeamCompeting.TeamName}");
                        }
                    }
                    else
                    {
                        output.Append("Matchup Not Yet Determined");
                        break;
                    }
                }

                return output.ToString();
            }
        }
    }
}