using System.Security.Cryptography;
using System.Text;

namespace Sportex.WebApi.Helpers;

public static class RazorpaySignatureHelper
{
    public static bool Verify(string payload, string actualSignature, string secret)
    {
        var key = Encoding.UTF8.GetBytes(secret);
        using var hmac = new HMACSHA256(key);
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        var generated = BitConverter.ToString(hash).Replace("-", "").ToLower();

        return generated == actualSignature;
    }
}
