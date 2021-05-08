using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IS_Turizmas.Models
{
    public partial class Route_PlaceOfInterest
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Route_id { get; set; }
        public int PlaceOfInterest_id { get; set; }


        public virtual Route Route_idNavigation { get; set; }
        public virtual PlaceOfInterest PlaceOfInterest_idNavigation { get; set; }
    }
}
