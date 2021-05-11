using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IS_Turizmas.Models
{
    public partial class ClientRoute
    {
        public DateTime? Start_date { get; set; }
        public DateTime? Finish_date { get; set; }
        [Required(ErrorMessage = "Laukas yra privalomas")]
        public int State_Id { get; set; }
        public DateTime? Calendar_date { get; set; }
        [Required(ErrorMessage = "Laukas yra privalomas")]
        public int Route_id { get; set; }
        public int? Item_id { get; set; }
        [Required(ErrorMessage = "Laukas yra privalomas")]
        public int Id { get; set; }

        public virtual ClientRouteState State_IdNavigation { get; set; }
        public virtual Route Route_idNavigation { get; set; }
        public virtual PersonalRouteItem Item_idNavigation { get; set; }
        /*
         * Client table is not realised yet. Uncomment, when client
         * table will be realised and needed.
         */
        //public virtual Client ClientNavigation { get; set; }
    }
}
