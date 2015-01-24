namespace PineTree.Language.Syntax
{
    public class ElseStatement : Statement
    {
        public SyntaxNode Body { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.ElseStatement;

        public ElseStatement(SyntaxNode body)
        {
            Body = body;
        }
    }
}