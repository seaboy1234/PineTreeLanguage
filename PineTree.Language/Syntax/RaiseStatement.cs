using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class RaiseStatement : Statement
    {
        public Expression Expression { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.RaiseStatement;

        public RaiseStatement(Expression expression)
        {
            Expression = expression;
        }
    }
}