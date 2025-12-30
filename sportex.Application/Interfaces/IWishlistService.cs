using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sportex.Application.DTOs.Wishlist;

namespace Sportex.Application.Interfaces;

public interface IWishlistService
{
    Task AddAsync(AddToWishlistDto dto);
    Task<IEnumerable<WishlistItemDto>> GetAsync(int userId);
    Task RemoveAsync(int id);
}
