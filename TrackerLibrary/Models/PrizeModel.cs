﻿using System.ComponentModel.DataAnnotations;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents what the prize is for the given place.
    /// </summary>
    public class PrizeModel
    {
        /// <summary>
        /// The unique identifier for the prize.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The numeric identifier for the place (2 for second place, etc.)
        /// </summary>
        [Required]
        [Display(Name = "Place Number")]
        [Range(1, 100)]
        public int PlaceNumber { get; set; }

        /// <summary>
        /// The friendly name for the place (second place, first runner up, etc.)
        /// </summary>
        [Required]
        [Display(Name = "Place Name")]
        [StringLength(100, MinimumLength = 3)]
        public string PlaceName { get; set; }

        /// <summary>
        /// The fixed amound this place earns or zero if it is not used.
        /// </summary>
        [Display(Name = "Prize Amount")]
        [DataType(DataType.Currency)]
        public decimal PrizeAmount { get; set; }

        /// <summary>
        /// The number that represents the percentage of the overall take or zero
        /// if it is not used. The percentage is a fraction of 1 (so 0.5 for 50%).
        /// </summary>
        [Display(Name = "Prize Percentage")]
        public double PrizePercentage { get; set; }

        public PrizeModel()
        {

        }
        public PrizeModel(string placeNumber, string placeName, string prizeAmount, string prizePercentage)
        {
            int placeNumberValue = 0;
            int.TryParse(placeNumber, out placeNumberValue);
            PlaceNumber = placeNumberValue;

            PlaceName = placeName;

            decimal prizeAmountValue = 0;
            decimal.TryParse(prizeAmount, out prizeAmountValue);
            PrizeAmount = prizeAmountValue;

            double prizePercentageValue = 0;
            double.TryParse(prizePercentage, out prizePercentageValue);
            PrizePercentage = prizePercentageValue;
        }
    }
}