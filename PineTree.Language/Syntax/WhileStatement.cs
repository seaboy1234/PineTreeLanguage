using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class WhileStatement : Statement
    {
        public SyntaxNode Body { get; }

        public Expression Predicate { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.WhileStatement;

        public WhileStatement(Expression predicate, SyntaxNode body)
        {
            Predicate = predicate;
            Body = body;
        }
    }
}