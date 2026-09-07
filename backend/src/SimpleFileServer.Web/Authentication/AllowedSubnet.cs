using System.Diagnostics.CodeAnalysis;
using System.Net;

namespace SimpleFileServer.Web.Authentication;

internal record class AllowedSubnet(
    byte[] SubnetAddressBytes,
    int PrefixLength
)
{
    public static bool TryParse([NotNullWhen(true)] string? rawValue, [MaybeNullWhen(false)] out AllowedSubnet result)
    {
        result = null;

        if (rawValue is null)
        {
            return false;
        }

        string[] parts = rawValue.Split('/', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 2
            || !IPAddress.TryParse(parts[0], out IPAddress? parsedAddress)
            || !int.TryParse(parts[1], out int prefixLength))
        {
            return false;
        }

        byte[] addressBytes = parsedAddress.GetAddressBytes();
        if (prefixLength > addressBytes.Length * 8)
        {
            // Prefix can't be longer than address length
            return false;
        }

        result = new(addressBytes, prefixLength);
        return true;
    }

    public bool DoesIncludeAddress(Span<byte> addressBytes)
    {
        if (addressBytes.Length != SubnetAddressBytes.Length)
        {
            return false;
        }

        int remainingPrefixLength = PrefixLength;
        int index = 0;
        do
        {
            // The mask would look like 11100000 (example for remainingPrefixLength = 3)
            byte prefixMask = (byte)(0xFF << (8 - Math.Min(remainingPrefixLength, 8)));
            // Get only meanigfull part
            byte addressBytePart = (byte)(addressBytes[index] & prefixMask);
            byte subnetAddressBytePart = (byte)(SubnetAddressBytes[index] & prefixMask);

            if (addressBytePart != subnetAddressBytePart)
            {
                return false;
            }

            index++;
            remainingPrefixLength -= 8;
        } while (index < addressBytes.Length && remainingPrefixLength > 0);

        return true;
    }

    public override string ToString()
    {
        var ipAddress = new IPAddress(SubnetAddressBytes);
        return $"{ipAddress}/{PrefixLength}";
    }
}
