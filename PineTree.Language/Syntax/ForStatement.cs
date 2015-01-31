using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class ForStatement : Statement
    {
        public SyntaxNode Body { get; }

        public Expression Increment { get; }

        public Expression Predicate { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.ForStatement;

        public SyntaxNode Variable { get; }

        public ForStatement(SyntaxNode variable, Expression predicate, Expression increment, SyntaxNode body)
        {
            Variable = variable;
            Predicate = predicate;
            Increment = increment;
            Body = body;
        }
    }
}