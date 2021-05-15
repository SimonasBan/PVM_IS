using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IS_Turizmas.Models
{
    public partial class OrientationGame
    {
        public OrientationGame()
        {
            ClientOrientationGame = new HashSet<ClientOrientationGame>();
            OrientationGame_Riddle = new HashSet<OrientationGame_Riddle>();
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Points_For_Completion { get; set; }

        public virtual ICollection<OrientationGame_Riddle> OrientationGame_Riddle { get; set; }
        public virtual ICollection<ClientOrientationGame> ClientOrientationGame { get; set; }
    }
}
