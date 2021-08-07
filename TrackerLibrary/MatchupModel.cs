using System.Collections.Generic;

namespace TrackerLibrary
{
    /// <summary>
    /// Represents one match int the tournament.
    /// </summary>
    public class MatchupModel
    {
        /// <summary>
        /// Which round this match is a part of.
        /// </summary>
        public int MatchupRound { get; set; }

        /// <summary>
        /// The winner of the match.
        /// </summary>
        public TeamModel Winner { get; set; }

        /// <summary>
        /// The set of teams that were involved in this match.
        /// </summary>
        public List<MatchupEntryModel> Entries { get; set; } = new List<MatchupEntryModel>();
    }
}