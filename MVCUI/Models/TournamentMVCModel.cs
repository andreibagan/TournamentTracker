using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace MVCUI.Models
{
    public class TournamentMVCModel
    {
        [Display(Name = "Tournament Name")]
        [StringLength(100, MinimumLength = 3)]
        [Required]
        public string TournamentName { get; set; }

        [Display(Name = "Entry Fee")]
        [Range(0, double.MaxValue)]
        [DataType(DataType.Currency)]
        [Required]
        public decimal EntryFee { get; set; }

        [Display(Name = "Entered Teams")]
        public List<SelectListItem> EnteredTeams { get; set; } = new List<SelectListItem>();

        public List<string> SelectedEnteredTeams { get; set; } = new List<string>();

        [Display(Name = "Prizes")]
        public List<SelectListItem> Prizes { get; set; } = new List<SelectListItem>();

        public List<string> SelectedPrizes { get; set; } = new List<string>();
    }
}