using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public enum ConstantType
    {
        String,
        Int,
        Float,
        Boolean,
        Null
    }

    public class ConstantExpression : Expression
    {
        public override SyntaxTypes SyntaxType => SyntaxTypes.ConstantExpression;

        public ConstantType Type { get; }

        public string Value { get; }

        public ConstantExpression(string value, ConstantType type)
        {
            Value = value;
            Type = type;
        }
    }
}