using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace IAMS.Model
{
    public class Country
    {
        public int CountryId { get; set; }
        [Required]
        public string? CountryName { get; set; }
        [Required, StringLength(3)]
        public string? CountryCode { get; set; }
        public DateTime? CreatedOn { get; set; }
        public DateTime? ModifiedOn { get; set; }
    }
}