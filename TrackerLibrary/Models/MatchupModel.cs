using System.Collections.Generic;
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
        /// Which round this match is a part of.
        /// </summary>
        public int MatchupRound { get; set; }

        /// <summary>
        /// The winner of the match.
        /// </summary>
        [Property("Id")]
        public TeamModel Winner { get; set; }

        /// <summary>
        /// The set of teams that were involved in this match.
        /// </summary>
        [ListDefined]
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
    }
}