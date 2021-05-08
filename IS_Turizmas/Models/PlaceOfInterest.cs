using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IS_Turizmas.Models
{
    public partial class PlaceOfInterest 
    {
        public PlaceOfInterest()
        {
            Route_PlaceOfInterest = new HashSet<Route_PlaceOfInterest>();
        }


        [Required(ErrorMessage = "Laukas yra privalomas")]
        public string Pavadinimas { get; set; }
        public string? Aprasymas { get; set; }
        public string? Miestas { get; set; }
        public string? Savivaldybe { get; set; }
        public string? Koordinates { get; set; }
        public string? Adresas { get; set; }
        public double? Bilieto_kaina { get; set; }
        public int? Taskai { get; set; }


        [Required(ErrorMessage = "Laukas yra privalomas")]
        public int Id { get; set; }


        public virtual ICollection<Route_PlaceOfInterest> Route_PlaceOfInterest { get; set; }
    }
}
