namespace PineTree.Language.Syntax
{
    public class PropertyDeclaration : SyntaxNode
    {
        public MethodDeclaration GetMethod { get; }

        public string Name { get; }

        public MethodDeclaration SetMethod { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.PropertyDeclaration;

        public string Type { get; }

        public Visibility Visibility { get; }

        public PropertyDeclaration(string name, string type, Visibility visibility, MethodDeclaration getMethod, MethodDeclaration setMethod)
        {
            Name = name;
            Type = type;
            Visibility = visibility;
            GetMethod = getMethod;
            SetMethod = setMethod;
        }
    }
}