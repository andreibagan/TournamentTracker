using System.ComponentModel.DataAnnotations;

namespace TrackerLibrary.Models
{
    /// <summary>
    /// Represents one person.
    /// </summary>
    public class PersonModel
    {
        /// <summary>
        /// The unique identifier for the person.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The first name of the person. 
        /// </summary>
        [Display(Name = "First Name")]
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string FirstName { get; set; }

        /// <summary>
        /// The last name of the person.
        /// </summary>
        [Display(Name = "Last Name")]
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string LastName { get; set; }

        /// <summary>
        /// The primary email address of the person.
        /// </summary>
        [Display(Name = "Email Address")]
        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(200, MinimumLength = 6)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// The primary cell phone number of the person.
        /// </summary>
        [Display(Name = "Cellphone Number")]
        [Required]
        [DataType(DataType.PhoneNumber)]
        [StringLength(20, MinimumLength = 7)]
        public string CellphoneNumber { get; set; }

        public string FullName
        {
            get
            {
                return $"{FirstName} {LastName}";
            }
        }
    }
}