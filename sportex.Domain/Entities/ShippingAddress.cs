using Sportex.Domain.Entities;

public class ShippingAddress
{
    public int Id { get; set; }
    public int UserId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? AltPhone { get; set; }
    public string AddressLine { get; set; } = string.Empty;
    public string? Landmark { get; set; }
    public string City { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public string Pincode { get; set; } = string.Empty;

    public bool IsDefault { get; set; }

    // 👉 Navigation property can stay nullable (this is fine)
    public User? User { get; set; }
}
