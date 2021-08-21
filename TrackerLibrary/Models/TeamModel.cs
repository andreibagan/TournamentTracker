using System.Collections.Generic;
using TournamentTracker.Attributes;

namespace TrackerLibrary.Models
{
    public class TeamModel
    {
        public int Id { get; set; }
        public string TeamName { get; set; }

        [RemoveProperty]
        public List<PersonModel> TeamMembers { get; set; } = new List<PersonModel>();
    }
}
