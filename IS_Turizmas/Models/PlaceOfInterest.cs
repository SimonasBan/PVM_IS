using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IS_Turizmas.Models
{
    public partial class PlaceOfInterest 
    {
        [Required(ErrorMessage = "Laukas yra privalomas")]
        public string Pavadinimas { get; set; }
        [Required(ErrorMessage = "Laukas yra privalomas")]
        public string Aprasymas { get; set; }



        [Required(ErrorMessage = "Laukas yra privalomas")]
        public string Miestas { get; set; }
        [Required(ErrorMessage = "Laukas yra privalomas")]
        public string Savivaldybe { get; set; }
        [Required(ErrorMessage = "Laukas yra privalomas")]
        public string Koordinates { get; set; }
        [Required(ErrorMessage = "Laukas yra privalomas")]
        public string Adresas { get; set; }
        //[Required(ErrorMessage = "Laukas yra privalomas")]
        //public TimeSpan Atsidarymo_laikas { get; set; }
        //[Required(ErrorMessage = "Laukas yra privalomas")]
        //public TimeSpan Uzsidarymo_laikas { get; set; }
        [Required(ErrorMessage = "Laukas yra privalomas")]
        public double Bilieto_kaina { get; set; }
        [Required(ErrorMessage = "Laukas yra privalomas")]
        public int Taskai { get; set; }


        [Required(ErrorMessage = "Laukas yra privalomas")]
        public int Id { get; set; }
    }
}
