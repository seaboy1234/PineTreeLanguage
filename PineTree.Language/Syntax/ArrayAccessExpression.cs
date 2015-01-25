using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class ArrayAccessExpression : Expression
    {
        public Expression Index { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.ArrayAccessExpression;

        public string Target { get; }

        public ArrayAccessExpression(string target, Expression index)
        {
            Target = target;
            Index = index;
        }
    }
}