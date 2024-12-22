using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IAMS.Model
{
    public class Address
    {
        public int AddressId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select country")] 
        public int CountryId { get; set; }
        [NotMapped]
        public string? CountryName { get; set; }
        [Required]
        [Display(Name = "Recipient name")]
        public string? RecipientName { get; set; }

        [Required]
        //House No, Suite, Apartment, etc.
        [Display(Name = "Address line 1")]

        public string? AddressLine1 { get; set; }
        //Street Name, Direction, etc.
        [Required]
        [Display(Name = "Address line 2")]
        public string? AddressLine2 { get; set; }
        //PO Box
        [Display(Name = "Address line 3")]
        public string? AddressLine3 { get; set; }

        public List<AddressFormat>? AddressFormats{ get; set; }


        [Required(ErrorMessage ="City name is required.")]
        [Display(Name = "City")]
        public string? City { get; set; }
        public string? District { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Select state/ province")]
        public int StateId { get; set; }
        [NotMapped]
        public string? StateName { get; set; }

        [Required(ErrorMessage ="Postal code is required.")]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "Select correct postal code.")]
        [Display(Name = "Postal code")]
        public string? PostalCode { get; set; }
        public string? AdditionalInfo { get; set; }
    }
}
