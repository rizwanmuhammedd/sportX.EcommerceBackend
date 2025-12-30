using Sportex.Application.DTOs.Checkout;

namespace Sportex.Application.Interfaces;

public interface ICheckoutService
{
    Task<IEnumerable<CheckoutPreviewDto>> PreviewAsync(int userId);
    Task ConfirmAsync(ConfirmCheckoutDto dto);
}
