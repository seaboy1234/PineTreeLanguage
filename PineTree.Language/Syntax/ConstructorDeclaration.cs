using System.Collections;
using System.Collections.Generic;

namespace PineTree.Language.Syntax
{
    public class ConstructorDeclaration : SyntaxNode
    {
        public IEnumerable<VariableDeclaration> Arguments { get; }

        public LexicalScope Body { get; }

        public string Name { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.ConstructorDeclaration;

        public Visibility Visibility { get; }

        public ConstructorDeclaration(string name, Visibility visibility, IEnumerable<VariableDeclaration> arguments, LexicalScope body)
        {
            Name = name;
            Visibility = visibility;
            Arguments = arguments;
            Body = body;
        }
    }
}