using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IS_Turizmas.Models
{
    public partial class OrientationGame_Riddle
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int OrientationGame_Id { get; set; }
        public int Riddle_Id { get; set; }


        public virtual OrientationGame OrientationGame_IdNavigation { get; set; }
        public virtual Riddle Riddle_IdNavigation { get; set; }
    }
}
