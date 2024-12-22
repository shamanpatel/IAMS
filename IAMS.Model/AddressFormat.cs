using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMS.Model
{
    public class AddressFormat
    {
        public int AddressFormatId { get; set; }
        public int CountryId { get; set; }
        public string? ComponentName { get; set; }
        public int ComponentOrder { get; set; }
        public Boolean IsRequired { get; set; }
    }
}
