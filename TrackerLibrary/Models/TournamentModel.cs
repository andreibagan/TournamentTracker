using System;
using System.Collections.Generic;
using TournamentTracker.Attributes;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one tournament, with all of the rounds, matchups, prizes and outcomes.
    /// </summary>
    public class TournamentModel
    {
        public event EventHandler<DateTime> OntournamentComplete;

        /// <summary>
        /// The unique identifier for the tournament.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// The name given to this tournament.
        /// </summary>
        public string TournamentName { get; set; }

        /// <summary>
        /// The amount of money each team needs to put up to enter.
        /// </summary>
        public decimal EntryFee { get; set; }

        /// <summary>
        /// The set of teams that have been entered.
        /// </summary>
        [RemoveProperty]
        public List<TeamModel> EnteredTeams { get; set; } = new List<TeamModel>();

        /// <summary>
        /// The list of prizes for the various places.
        /// </summary>
        [RemoveProperty]
        public List<PrizeModel> Prizes { get; set; } = new List<PrizeModel>();

        /// <summary>
        /// the matchups per round.
        /// </summary>
        [RemoveProperty]
        public List<List<MatchupModel>> Rounds { get; set; } = new List<List<MatchupModel>>();

        public void CompleteTournament()
        {
            OntournamentComplete?.Invoke(this, DateTime.Now);
        }
    }
}
