using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IS_Turizmas.Models
{
    public partial class PlaceOfInterestComment
    {

        [Required(ErrorMessage = "Laukas yra privalomas")]
        public string Comment { get; set; }
        public int? Rating { get; set; }
        public int? PlaceOfInterest_Id { get; set; }


        [Required(ErrorMessage = "Laukas yra privalomas")]
        public int Id { get; set; }
    }
}

