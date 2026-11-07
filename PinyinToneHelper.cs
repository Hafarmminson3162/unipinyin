using System;
using System.Text;

namespace UniPinyin
{
    public static class PinyinToneHelper
    {
        // Tone mark arrays: index 0: a, 1: e, 2: i, 3: o, 4: u, 5: ü
        private static readonly char[] Tone1 = { '\u0101', '\u0113', '\u012b', '\u014d', '\u016b', '\u01d6' };
        private static readonly char[] Tone2 = { '\u00e1', '\u00e9', '\u00ed', '\u00f3', '\u00fa', '\u01d8' };
        private static readonly char[] Tone3 = { '\u01ce', '\u011b', '\u01d0', '\u01d2', '\u01d4', '\u01da' };
        private static readonly char[] Tone4 = { '\u00e0', '\u00e8', '\u00ec', '\u00f2', '\u00f9', '\u01dc' };
        private static readonly char[] BaseV = { 'a', 'e', 'i', 'o', 'u', '\u00fc' };

        public static string ProcessKey(string currentBuffer, char c, out bool handled)
        {
            handled = false;
            c = char.ToLowerInvariant(c);

            if (string.IsNullOrEmpty(currentBuffer))
            {
                return c.ToString();
            }

            // Smart continuation: If the IMMEDIATELY preceding character was given a tone
            // and the current key forms an initial consonant (x+i/u, s+h/vowel, r+vowel, f+vowel),
            // revert the tone back to consonant and append current character.
            int lastCharIdx = currentBuffer.Length - 1;
            char lastChar = currentBuffer[lastCharIdx];
            int lastCharTone = GetCurrentTone(lastChar);
            if (lastCharTone > 0)
            {
                bool revert = false;
                char consonant = ' ';

                if (lastCharTone == 1 && (c == 'i' || c == 'u'))
                {
                    revert = true; consonant = 'x';
                }
                else if (lastCharTone == 2 && (c == 'h' || IsVowel(c)))
                {
                    revert = true; consonant = 's';
                }
                else if (lastCharTone == 3 && IsVowel(c))
                {
                    revert = true; consonant = 'r';
                }
                else if (lastCharTone == 4 && IsVowel(c))
                {
                    revert = true; consonant = 'f';
                }

                if (revert)
                {
                    char baseV = GetBaseVowel(lastChar);
                    StringBuilder sb = new StringBuilder(currentBuffer);
                    sb[lastCharIdx] = baseV;
                    sb.Append(consonant);
                    sb.Append(c);
                    handled = true;
                    return sb.ToString();
                }
            }

            // 1. Rule for 'ü' (w or u+w):
            if (c == 'w')
            {
                // If previous char is 'ü', typing 'w' again gives "uw"
                if (currentBuffer.EndsWith("\u00fc", StringComparison.OrdinalIgnoreCase))
                {
                    handled = true;
                    return currentBuffer.Substring(0, currentBuffer.Length - 1) + "uw";
                }

                // 'u' + 'w' -> 'ü'
                if (currentBuffer.EndsWith("u", StringComparison.OrdinalIgnoreCase))
                {
                    handled = true;
                    return currentBuffer.Substring(0, currentBuffer.Length - 1) + "\u00fc";
                }

                // 'lw' -> 'lü', 'nw' -> 'nü'
                if (currentBuffer.EndsWith("l", StringComparison.OrdinalIgnoreCase) ||
                    currentBuffer.EndsWith("n", StringComparison.OrdinalIgnoreCase))
                {
                    handled = true;
                    return currentBuffer + "\u00fc";
                }
            }

            // 2. Rule for tones: x=1, s=2, r=3, f=4
            int tone = 0;
            if (c == 'x') tone = 1;
            else if (c == 's') tone = 2;
            else if (c == 'r') tone = 3;
            else if (c == 'f') tone = 4;

            if (tone > 0)
            {
                int targetIdx = FindVowelIndexForTone(currentBuffer);
                if (targetIdx >= 0)
                {
                    char targetChar = currentBuffer[targetIdx];
                    int currentTone = GetCurrentTone(targetChar);
                    char baseVowel = GetBaseVowel(targetChar);

                    if (currentTone == tone)
                    {
                        // Repeat key undo: revert tone AND append original key
                        handled = true;
                        StringBuilder sb = new StringBuilder(currentBuffer);
                        sb[targetIdx] = baseVowel;
                        sb.Append(c);
                        return sb.ToString();
                    }
                    else
                    {
                        // Apply or change tone
                        handled = true;
                        char newChar = ApplyTone(baseVowel, tone);
                        StringBuilder sb = new StringBuilder(currentBuffer);
                        sb[targetIdx] = newChar;
                        return sb.ToString();
                    }
                }
            }

            return currentBuffer + c;
        }

        private static int FindVowelIndexForTone(string buffer)
        {
            int start = buffer.Length - 1;
            int lastVowel = -1;
            int aIdx = -1;
            int eIdx = -1;
            int ouIdx = -1;

            bool seenVowel = false;
            bool seenConsonant = false;

            for (int i = start; i >= 0; i--)
            {
                char ch = buffer[i];
                char baseCh = GetBaseVowel(ch);

                if (IsVowel(baseCh))
                {
                    if (seenConsonant)
                    {
                        break;
                    }
                    seenVowel = true;
                    if (lastVowel == -1) lastVowel = i;
                    if (baseCh == 'a') aIdx = i;
                    if (baseCh == 'e') eIdx = i;
                    if (baseCh == 'o' && i + 1 < buffer.Length && GetBaseVowel(buffer[i + 1]) == 'u')
                    {
                        ouIdx = i;
                    }
                }
                else
                {
                    if (seenVowel)
                    {
                        seenConsonant = true;
                    }
                    if (ch == ' ' || ch == '\'')
                    {
                        break;
                    }
                }
            }

            if (aIdx >= 0) return aIdx;
            if (eIdx >= 0) return eIdx;
            if (ouIdx >= 0) return ouIdx;
            return lastVowel;
        }

        public static bool IsVowel(char c)
        {
            c = char.ToLowerInvariant(c);
            return c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u' || c == '\u00fc';
        }

        public static char GetBaseVowel(char c)
        {
            c = char.ToLowerInvariant(c);
            for (int i = 0; i < 6; i++)
            {
                if (c == Tone1[i] || c == Tone2[i] || c == Tone3[i] || c == Tone4[i] || c == BaseV[i])
                {
                    return BaseV[i];
                }
            }
            return c;
        }

        public static int GetCurrentTone(char c)
        {
            c = char.ToLowerInvariant(c);
            for (int i = 0; i < 6; i++)
            {
                if (c == Tone1[i]) return 1;
                if (c == Tone2[i]) return 2;
                if (c == Tone3[i]) return 3;
                if (c == Tone4[i]) return 4;
            }
            return 0;
        }

        public static char ApplyTone(char baseVowel, int tone)
        {
            baseVowel = char.ToLowerInvariant(baseVowel);
            int vIdx = -1;
            for (int i = 0; i < 6; i++)
            {
                if (BaseV[i] == baseVowel) { vIdx = i; break; }
            }
            if (vIdx < 0) return baseVowel;

            switch (tone)
            {
                case 1: return Tone1[vIdx];
                case 2: return Tone2[vIdx];
                case 3: return Tone3[vIdx];
                case 4: return Tone4[vIdx];
                default: return BaseV[vIdx];
            }
        }

        public static string StripTones(string pinyin)
        {
            if (string.IsNullOrEmpty(pinyin)) return "";
            StringBuilder sb = new StringBuilder();
            foreach (char c in pinyin)
            {
                char baseV = GetBaseVowel(c);
                if (baseV == '\u00fc') sb.Append('v'); // Convert ü to v for dictionary & Google API lookup
                else sb.Append(baseV);
            }
            return sb.ToString();
        }
    }
}
