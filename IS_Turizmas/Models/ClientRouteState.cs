using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IS_Turizmas.Models
{
    public partial class ClientRouteState
    {
        public ClientRouteState()
        {
            ClientRoute = new HashSet<ClientRoute>();
        }

        public int Id { get; set; }
        public string State { get; set; }

        public virtual ICollection<ClientRoute> ClientRoute { get; set; }
    }
}
