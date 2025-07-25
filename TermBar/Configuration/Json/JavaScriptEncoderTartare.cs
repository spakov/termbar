using System.Text;
using System.Text.Encodings.Web;

namespace Spakov.TermBar.Configuration.Json
{
    /// <summary>
    /// A nearly raw JSON encoder.
    /// </summary>
    /// <remarks>See <see cref="Encode"/> for implementation guts.</remarks>
    internal class JavaScriptEncoderJsonTartare : JavaScriptEncoder
    {
        /// <summary>
        /// The singleton instance.
        /// </summary>
        public static JavaScriptEncoderJsonTartare Instance { get; } = new();

        public override int MaxOutputCharactersPerInputCharacter => 6;

        public override unsafe int FindFirstCharacterToEncode(char* text, int textLength)
        {
            for (int i = 0; i < textLength; i++)
            {
                char c = text[i];

                if (WillEncode(c))
                {
                    return i;
                }
                else
                {
                    if (char.IsHighSurrogate(c))
                    {
                        if (i + 1 < textLength && char.IsLowSurrogate(text[i + 1]))
                        {
                            i++;
                            continue;
                        }
                        else
                        {
                            return i;
                        }
                    }
                }
            }

            return -1;
        }

        public override unsafe bool TryEncodeUnicodeScalar(int unicodeScalar, char* buffer, int bufferLength, out int numberOfCharactersWritten)
        {
            Rune rune = new(unicodeScalar);
            string encoded = rune.Value < 0x10000 && WillEncode((char)rune.Value)
                ? Encode((char)rune.Value)
                : rune.ToString();

            if (encoded.Length > bufferLength)
            {
                numberOfCharactersWritten = 0;
                return false;
            }

            for (int i = 0; i < encoded.Length; i++)
            {
                buffer[i] = encoded[i];
            }

            numberOfCharactersWritten = encoded.Length;
            return true;
        }

        public override bool WillEncode(int unicodeScalar) => unicodeScalar is < 0x20 or '\\' or '"';

        /// <summary>
        /// Initializes a <see cref="JavaScriptEncoderJsonTartare"/>.
        /// </summary>
        private JavaScriptEncoderJsonTartare() {
        }

        /// <summary>
        /// Replaces raw characters with their respective JSON-escaped
        /// entities.
        /// </summary>
        /// <remarks>
        /// <para>This accomplishes a few goals:</para>
        /// <list type="bullet">
        /// <item>Keeps fancy Unicode characters like  and  easily
        /// visible.</item>
        /// <item>Ensures whitespace is readily identifiable.</item>
        /// <item>Complies with RFC 7159.</item>
        /// </list>
        /// </remarks>
        /// <param name="c">The character to encode.</param>
        /// <returns>The encoded version of <paramref name="c"/>.</returns>
        private static string Encode(char c)
        {
            return c switch
            {
                '\\' => @"\\",
                '"' => "\\\"",
                (char)0x00 => @"\u0000",
                (char)0x01 => @"\u0001",
                (char)0x02 => @"\u0002",
                (char)0x03 => @"\u0003",
                (char)0x04 => @"\u0004",
                (char)0x05 => @"\u0005",
                (char)0x06 => @"\u0006",
                '\a' => @"\u0007",
                '\b' => @"\b",
                '\t' => @"\t",
                '\n' => @"\n",
                '\v' => @"\u000b",
                '\f' => @"\f",
                '\r' => @"\r",
                (char)0x0e => @"\u000e",
                (char)0x0f => @"\u000f",
                (char)0x10 => @"\u0010",
                (char)0x11 => @"\u0011",
                (char)0x12 => @"\u0012",
                (char)0x13 => @"\u0013",
                (char)0x14 => @"\u0014",
                (char)0x15 => @"\u0015",
                (char)0x16 => @"\u0016",
                (char)0x17 => @"\u0017",
                (char)0x18 => @"\u0018",
                (char)0x19 => @"\u0019",
                (char)0x1a => @"\u001a",
                (char)0x1b => @"\u001b",
                (char)0x1c => @"\u001c",
                (char)0x1d => @"\u001d",
                (char)0x1e => @"\u001e",
                (char)0x1f => @"\u001f",
                _ => c.ToString()
            };
        }
    }
}
