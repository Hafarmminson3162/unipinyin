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

            // Dấu nháy đơn ' dùng để ngăn cách giữa các từ/âm tiết (VD: xi'an, wo'shi)
            if (c == '\'')
            {
                if (!string.IsNullOrEmpty(currentBuffer) && !currentBuffer.EndsWith("'"))
                {
                    handled = true;
                    return currentBuffer + "'";
                }
                return currentBuffer;
            }

            if (string.IsNullOrEmpty(currentBuffer))
            {
                // Phím 'w' khi đứng một mình tạo âm 'ü'
                if (c == 'w')
                {
                    handled = true;
                    return "\u00fc";
                }
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

            // 1. Quy tắc âm 'ü' (w hoặc u+w):
            if (c == 'w')
            {
                // Nếu đi sau dấu cách hoặc dấu nháy đơn '
                if (currentBuffer.EndsWith(" ") || currentBuffer.EndsWith("'"))
                {
                    handled = true;
                    return currentBuffer + "\u00fc";
                }

                // Khi ấn 2 lần w (ký tự cuối là 'ü' hoặc ü có dấu): SẼ HIỆN RA 'w' CHỨ KHÔNG PHẢI 'uw'
                int lastIdx = currentBuffer.Length - 1;
                char lastCh = currentBuffer[lastIdx];
                if (GetBaseVowel(lastCh) == '\u00fc')
                {
                    handled = true;
                    return currentBuffer.Substring(0, lastIdx) + "w";
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

            // Nếu ký tự cuối là 'ü' (đứng đầu từ hoặc sau dấu ') và theo sau là a, o, e, u:
            // tự động chuyển ü về w để gõ được các âm bắt đầu bằng w (VD: 'wo', 'wa', 'wen', 'wu')
            if (currentBuffer.EndsWith("\u00fc", StringComparison.OrdinalIgnoreCase))
            {
                bool isAfterLOrN = currentBuffer.Length >= 2 &&
                    (currentBuffer[currentBuffer.Length - 2] == 'l' || currentBuffer[currentBuffer.Length - 2] == 'n');
                if (!isAfterLOrN && (c == 'a' || c == 'o' || c == 'e' || c == 'u'))
                {
                    return currentBuffer.Substring(0, currentBuffer.Length - 1) + "w" + c;
                }
            }

            // 2. Quy tắc bỏ dấu thanh điệu Pinyin kiểu TELEX:
            // x: Thanh 1 (Thanh ngang / macaron)
            // s: Thanh 2 (Dấu sắc)
            // r: Thanh 3 (Dấu hỏi / caron)
            // f: Thanh 4 (Dấu huyền / grave)
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
                        // Nhấn lại cùng phím dấu: Hủy dấu và xuất chữ gốc (Repeat key undo giống UniKey)
                        handled = true;
                        StringBuilder sb = new StringBuilder(currentBuffer);
                        sb[targetIdx] = baseVowel;
                        sb.Append(c);
                        return sb.ToString();
                    }
                    else
                    {
                        // Thêm dấu mới hoặc đổi sang dấu khác
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

            // Quét ngược tìm nguyên âm của âm tiết hiện tại
            for (int i = start; i >= 0; i--)
            {
                char ch = buffer[i];
                char baseCh = GetBaseVowel(ch);

                if (IsVowel(baseCh))
                {
                    if (seenConsonant)
                    {
                        // Đã chạm sang âm tiết trước đó
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

            // Quy tắc đặt dấu thanh Pinyin quốc tế:
            // 1. Ưu tiên 'a', rồi đến 'e'
            // 2. Trong vần 'ou' đặt ở 'o'
            // 3. Các trường hợp khác đặt ở nguyên âm cuối
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
                if (baseV == '\u00fc') sb.Append('v'); // Chuyển ü thành v để tra từ điển & Google API
                else sb.Append(baseV);
            }
            return sb.ToString();
        }
    }
}
