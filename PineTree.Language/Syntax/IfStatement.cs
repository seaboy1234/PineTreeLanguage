using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class IfStatement : Statement
    {
        public SyntaxNode Body { get; }

        public ElseStatement ElseStatement { get; }

        public Expression Predicate { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.IfStatement;

        public IfStatement(Expression predicate, SyntaxNode body, ElseStatement elseStatement)
        {
            Predicate = predicate;
            Body = body;
            ElseStatement = elseStatement;
        }
    }
}