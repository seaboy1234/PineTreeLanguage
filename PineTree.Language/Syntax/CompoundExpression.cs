using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class CompoundExpression : Expression
    {
        public IEnumerable<Expression> Expressions { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.CompoundExpression;

        public CompoundExpression(IEnumerable<Expression> expressions)
        {
            Expressions = expressions;
        }
    }
}