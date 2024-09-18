using System.Text;

namespace LdapAttributeValueRetriever.Worker;
internal static class OidHelper
{
    public static string ConvertHexToOid(string hex)
    {
        byte[] bytes = HexStringToByteArray(hex);
        StringBuilder oid = new StringBuilder();

        // Decode the first two numbers using (40 * x + y)
        int firstByte = bytes[0];
        int x = firstByte / 40;
        int y = firstByte % 40;
        oid.Append(x).Append('.').Append(y);

        // Decode the remaining bytes
        int value = 0;
        bool multiByte = false;

        for (int i = 1; i < bytes.Length; i++)
        {
            int currentByte = bytes[i];

            // Check if MSB is set (indicating a multi-byte value)
            if ((currentByte & 0x80) != 0)
            {
                // Remove MSB and shift the value
                value = (value << 7) | (currentByte & 0x7F);
                multiByte = true;
            }
            else
            {
                // If multi-byte sequence is ending, finalize the value
                if (multiByte)
                {
                    value = (value << 7) | currentByte;
                    oid.Append('.').Append(value);
                    value = 0;
                    multiByte = false;
                }
                else
                {
                    // Single byte value
                    oid.Append('.').Append(currentByte);
                }
            }
        }

        return oid.ToString();
    }

    static byte[] HexStringToByteArray(string hex)
    {
        int length = hex.Length;
        byte[] bytes = new byte[length / 2];

        for (int i = 0; i < length; i += 2)
        {
            bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }

        return bytes;
    }
}
