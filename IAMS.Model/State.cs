using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMS.Model
{
    public class State
    {
        public int StateId { get; set; }
        [Required]
        public int CountryId { get; set; }
        [Required]
        public string? StateName { get; set; }
        [NotMapped]
        public string? CountryName { get; set; }
        public string? StateCode { get; set; }
    }
}
