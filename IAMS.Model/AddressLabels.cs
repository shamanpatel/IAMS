using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAMS.Model
{
    public class AddressLabels
    {
        //public string? AddressLine1 { get; set; }
        //public string? AddressLine2 { get; set; }
        //public string? AddressLine3 { get; set; }
        public List<IAMS.Model.AddressFormat>? AddressFormats { get; set; }
        public List<IAMS.Model.State>? States { get; set; }

    }
}
