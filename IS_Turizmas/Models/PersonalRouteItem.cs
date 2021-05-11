using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IS_Turizmas.Models
{
    public partial class PersonalRouteItem
    {
        public PersonalRouteItem()
        {
            ClientRoute = new HashSet<ClientRoute>();
        }

        public string Item { get; set; }
        public int Id { get; set; }

        public virtual ICollection<ClientRoute> ClientRoute { get; set; }
    }
}
