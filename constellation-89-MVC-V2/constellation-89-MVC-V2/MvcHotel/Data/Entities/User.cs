using System;
using System.Collections.Generic;

namespace MvcHotel.Data.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
