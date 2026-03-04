using Sportex.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Sportex.Domain.Entities
{
    public class ReviewHelpful
    {
        public int Id { get; set; }
        public int ReviewId { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public Review? Review { get; set; }
        public User? User { get; set; }
    }
}

