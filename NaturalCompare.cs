using System;
using System.Collections.Generic;

namespace NaturalOrdering
{
    public enum StringSegmentResult
    {
        Over=0,
        Chara=1,
        Digit=2,
        CharaAndDigit=3,
        Space=4,
        CharaAndSpace = 5,
        DigitAndSpace =6
    }
    public class StringSegment
    {
        private readonly string value;
        public int current;
        private int start_index;
        public StringSegment(string value)
        {
            this.value = value;
            current = 0;
            start_index = 0;
        }
        public ReadOnlySpan<char> GetPart => value.AsSpan(start_index, current - start_index);

        public StringSegmentResult Check()
        {
            start_index = current;
            if (current == value.Length)
            {
                return StringSegmentResult.Over;
            }
            if (char.IsDigit(value[current]))
            {
                while (++current < value.Length && char.IsDigit(value[current])) ;
                return StringSegmentResult.Digit;
            }
            else if (char.IsWhiteSpace(value[current]))
            {
                while (++current < value.Length && char.IsWhiteSpace(value[current])) ;
                return StringSegmentResult.Space;
            }
            while (++current < value.Length && !char.IsDigit(value[current]) && !char.IsWhiteSpace(value[current])) ;
            return StringSegmentResult.Chara;
        }
    }
    public class NaturalCompare : IComparer<string>
    {
        public NaturalCompare(StringComparison comparison = StringComparison.Ordinal)
        {
            _comparison = comparison;
        }

        private readonly StringComparison _comparison;

        public int Compare(string? x, string? y)
        {
            if (x == null || y == null)
            {
                return string.Compare(x, y, StringComparison.Ordinal);
            }
            var xseg = new StringSegment(x);
            var yseg = new StringSegment(y);
            StringSegmentResult xresult, yresult;
            while (true)
            {
                xresult = xseg.Check();
                yresult = yseg.Check();
                //비교시작
                if (xresult == yresult)
                {
                    // 모두 탐색된 경우
                    if (xresult == StringSegmentResult.Over)
                    {
                        break;
                    }
                    switch (xresult)
                    {
                        case StringSegmentResult.Digit:
                            var xdigit = int.Parse(xseg.GetPart);
                            var ydigit = int.Parse(yseg.GetPart);
                            if (xdigit < ydigit) return -1;
                            else if (xdigit > ydigit) return 1;
                            break;

                        case StringSegmentResult.Chara:
                            int result = xseg.GetPart.CompareTo(yseg.GetPart, StringComparison.Ordinal);
                            if (result != 0) return result;
                            break;

                        case StringSegmentResult.Space:
                            if (xseg.GetPart.Length < yseg.GetPart.Length) return 1;
                            else if (xseg.GetPart.Length > yseg.GetPart.Length) return -1;
                            break;
                    }
                }
                switch (xresult | yresult)
                {
                    case StringSegmentResult.CharaAndDigit:
                        if (xresult == StringSegmentResult.Digit)
                        {
                            if (IsSpecialChar(yseg.GetPart[0])) return 1;
                            return -1;
                        }
                        if (IsSpecialChar(xseg.GetPart[0])) return -1;
                        return 1;

                    case StringSegmentResult.DigitAndSpace:
                    case StringSegmentResult.CharaAndSpace:
                        if (xresult == StringSegmentResult.Space) return -1;
                        return 1;
                }
                //어느 한쪽이 모두 탐색된 경우
                if (xresult == StringSegmentResult.Over) return -1;
                else if (yresult == StringSegmentResult.Over) return 1;
            }
            return 0;
        }

        public static int CompareOrdinal(string? x, string? y)
        {
            if (x == null || y == null)
            {
                return string.Compare(x, y, StringComparison.Ordinal);
            }
            var xseg = new StringSegment(x);
            var yseg = new StringSegment(y);
            StringSegmentResult xresult, yresult;
            while (true)
            {
                xresult = xseg.Check();
                yresult = yseg.Check();
                //비교시작
                if (xresult == yresult)
                {
                    // 모두 탐색된 경우
                    if (xresult == StringSegmentResult.Over)
                    {
                        break;
                    }
                    switch (xresult)
                    {
                        case StringSegmentResult.Digit:
                            var xdigit = int.Parse(xseg.GetPart);
                            var ydigit = int.Parse(yseg.GetPart);
                            if (xdigit < ydigit) return -1;
                            else if (xdigit > ydigit) return 1;
                            break;

                        case StringSegmentResult.Chara:
                            int result = xseg.GetPart.CompareTo(yseg.GetPart, StringComparison.Ordinal);
                            if (result != 0) return result;
                            break;

                        case StringSegmentResult.Space:
                            if (xseg.GetPart.Length < yseg.GetPart.Length) return 1;
                            else if (xseg.GetPart.Length > yseg.GetPart.Length) return -1;
                            break;
                    }
                }
                switch (xresult | yresult)
                {
                    case StringSegmentResult.CharaAndDigit:
                        if (xresult == StringSegmentResult.Digit)
                        {
                            if (IsSpecialChar(yseg.GetPart[0])) return 1;
                            return -1;
                        }
                        if (IsSpecialChar(xseg.GetPart[0])) return -1;
                        return 1;

                    case StringSegmentResult.DigitAndSpace:
                    case StringSegmentResult.CharaAndSpace:
                        if (xresult == StringSegmentResult.Space) return -1;
                        return 1;
                }
                //어느 한쪽이 모두 탐색된 경우
                if (xresult == StringSegmentResult.Over) return -1;
                else if (yresult == StringSegmentResult.Over) return 1;
            }
            return 0;
        }
        private static bool IsSpecialChar(char value)
        {
            return char.IsWhiteSpace(value) || char.IsPunctuation(value) || char.IsSymbol(value) || char.IsControl(value);
        }
    }
}
