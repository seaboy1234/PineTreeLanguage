using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class ReturnStatement : Statement
    {
        public Expression Expression { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.ReturnStatement;

        public ReturnStatement(Expression expression)
        {
            Expression = expression;
        }
    }
}