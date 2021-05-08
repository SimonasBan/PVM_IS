using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace IS_Turizmas.Models
{
    public partial class Route
    {
        public Route()
        {
            Route_PlaceOfInterest = new HashSet<Route_PlaceOfInterest>();
            ClientRoute = new HashSet<ClientRoute>();
        }

        public int? Length { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        //? is written by nullable fields
        public double? Rating { get; set; }
        public int Id { get; set; }

        public virtual ICollection<Route_PlaceOfInterest> Route_PlaceOfInterest { get; set; }
        public virtual ICollection<ClientRoute> ClientRoute { get; set; }
    }
}
