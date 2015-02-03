using System;

namespace PineTree.Language.Syntax
{
    public class CatchStatement : Statement
    {
        public SyntaxNode Body { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.CatchStatement;

        public VariableDeclaration Variable { get; }

        public CatchStatement(SyntaxNode body, VariableDeclaration variable)
        {
            Body = body;
        }
    }
}