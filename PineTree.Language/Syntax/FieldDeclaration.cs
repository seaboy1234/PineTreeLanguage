namespace PineTree.Language.Syntax
{
    public class FieldDeclaration : SyntaxNode
    {
        public string Name { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.FieldDeclaration;

        public string Type { get; }

        public Expression Value { get; }

        public FieldDeclaration(string name, string type, Expression value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}