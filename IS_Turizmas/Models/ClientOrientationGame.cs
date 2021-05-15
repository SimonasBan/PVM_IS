using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IS_Turizmas.Models
{
    public partial class ClientOrientationGame
    {
        public int Id { get; set; }
        public int OrientationGame_Id { get; set; }
        public int CurrentNumber { get; set; }
        public ClientOrientationGameState State { get; set; }

        public virtual OrientationGame OrientationGame_IdNavigation { get; set; }
    }

    public enum ClientOrientationGameState
    {
        InCodeSubmission = 1,
        InRiddleSubmission = 2,
        Ended = 3
    }
}
