using System;
using System.Collections.Generic;
using System.Web.UI;

namespace LteDev.RegexParsing
{
    public sealed class SimpleQualifierToken : QuantifierToken
    {
        public SimpleQualifierToken(QuantifierType type, bool isLazy)
            : base(type, isLazy)
        {
            switch (type)
            {
                case QuantifierType.Optional:
                case QuantifierType.OptionalRepeat:
                case QuantifierType.Multiple:
                    break;
                default:
                    throw new ArgumentOutOfRangeException("tokenType");
            }
        }

        public override IEnumerable<char> GetPattern()
        {
            switch (Type)
            {
                case QuantifierType.Optional:
                    yield return '?';
                    break;
                case QuantifierType.OptionalRepeat:
                    yield return '*';
                    break;
                default:
                    yield return '+';
                    break;
            }
            if (IsLazy)
                yield return '?';
        }

        public override void WriteTo(HtmlTextWriter writer, List<string> classNames, ICssClassMapper classMapper)
        {
            switch (Type)
            {
                case QuantifierType.Optional:
                    writer.Write(IsLazy ? "??" : "?");
                    break;
                case QuantifierType.OptionalRepeat:
                    writer.Write(IsLazy ? "*?" : "*");
                    break;
                default:
                    writer.Write(IsLazy ? "+?" : "+");
                    break;
            }
        }
    }
}
