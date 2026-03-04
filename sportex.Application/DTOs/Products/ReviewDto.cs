using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sportex.Application.DTOs.Products
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserAvatar { get; set; } = string.Empty;
        public string UserProfileImage { get; set; } = string.Empty; // ✅ ADDED THIS
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int HelpfulCount { get; set; }
        public bool IsVerifiedPurchase { get; set; }
        public bool IsHelpful { get; set; }
    }
}