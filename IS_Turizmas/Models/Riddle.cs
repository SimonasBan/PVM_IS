using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IS_Turizmas.Models
{
    public partial class Riddle
    {
        public Riddle()
        {
            OrientationGame_Riddle = new HashSet<OrientationGame_Riddle>();
        }
        public int Id { get; set; }
        public string RiddleQuestion { get; set; }
        public string Answer { get; set; }
        public string PlaceCode { get; set; }
        public string PlaceCodeAnswer { get; set; }

        public int PlaceOfInterest_Id { get; set; }

        public virtual PlaceOfInterest PlaceOfInterest_IdNavigation { get; set; }

        public virtual ICollection<OrientationGame_Riddle> OrientationGame_Riddle { get; set; }
    }
}
