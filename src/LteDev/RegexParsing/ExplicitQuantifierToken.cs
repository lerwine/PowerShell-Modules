using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace LteDev.RegexParsing
{
    public sealed class ExplicitQuantifierToken : QuantifierToken
    {
        public string MinValue { get; }

        public string MaxValue { get; }

        public ExplicitQuantifierToken(string fixedValue, bool isLazy) : base(QuantifierType.Fixed, isLazy)
        {
            if (string.IsNullOrEmpty(fixedValue))
                throw new ArgumentException("fixedValue cannot be null or empty", "fixedValue");
            if (!fixedValue.All(char.IsNumber))
                throw new ArgumentOutOfRangeException("fixedValue");
            MaxValue = "";
        }

        public ExplicitQuantifierToken(string minValue, string maxValue, bool isLazy) : base(string.IsNullOrEmpty(maxValue) ? QuantifierType.MinRepeat : QuantifierType.Limited, isLazy)
        {
            if (string.IsNullOrEmpty(minValue))
                throw new ArgumentException("minValue cannot be null or empty", "minValue");
            if (!minValue.All(char.IsNumber))
                throw new ArgumentOutOfRangeException("minValue");
            if (string.IsNullOrEmpty(maxValue))
                MaxValue = "";
            else
            {
                if (!maxValue.All(char.IsNumber))
                    throw new ArgumentOutOfRangeException("maxValue");
                MaxValue = maxValue;
            }
        }

        public override IEnumerable<char> GetPattern()
        {
            yield return '{';
            foreach (char c in MinValue)
                yield return c;
            switch (Type)
            {
                case QuantifierType.Fixed:
                    yield return '}';
                    break;
                case QuantifierType.MinRepeat:
                    yield return ',';
                    yield return '}';
                    break;
                default:
                    yield return ',';
                    foreach (char c in MaxValue)
                        yield return c;
                    yield return '}';
                    break;
            }
            if (IsLazy)
                yield return '?';
        }

        public override void WriteTo(Html32TextWriter writer, List<string> classNames, ICssClassMapper classMapper)
        {
            writer.Write('{');
            string[] spanClassNames;
            if (!classMapper.TryGetNameClassNames(out spanClassNames))
                spanClassNames = null;
            RegexParser.WriteSpanned(MinValue, writer, classNames, spanClassNames);
            switch (Type)
            {
                case QuantifierType.Limited:
                    writer.Write(",");
                    RegexParser.WriteSpanned(MaxValue, writer, classNames, spanClassNames);
                    break;
                case QuantifierType.MinRepeat:
                    writer.Write(",");
                    break;
            }
            writer.Write(IsLazy ? "}?" : "}");
        }
    }
}
