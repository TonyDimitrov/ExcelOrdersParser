using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Werehouse.Models
{
    public class CalculatedOrder
    {
        public DateTime RequestDate { get; set; } 
        public string RequestNumber { get; set; } 
        public string PharmacyName { get; set; }  
        public string Product { get; set; }       
        public int RequestedQuantity { get; set; }
        public int RabatQuantity { get; set; }
        public int PartialQuantity { get; set; }
        public bool FlagCompleted { get; set; }
        public bool FlagDalay { get; set; }
        public bool FlagPartiallyDone { get; set; }

        public bool IsValid { get; set; } = true;
    }
}
