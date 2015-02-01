using System.Collections;
using System.Collections.Generic;

namespace PineTree.Language.Syntax
{
    public class MethodDeclaration : SyntaxNode
    {
        public IEnumerable<VariableDeclaration> Arguments { get; }

        public LexicalScope Body { get; }

        public string Name { get; }

        public IEnumerable<PreconditionStatement> Preconditions { get; }

        public string ReturnType { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.MethodDeclaration;

        public Visibility Visibility { get; }

        public MethodDeclaration(string name, string returnType, Visibility visibility, IEnumerable<VariableDeclaration> arguments, LexicalScope body)
        {
            Name = name;
            ReturnType = returnType;
            Visibility = visibility;
            Arguments = arguments;
            Body = body;
        }

        public MethodDeclaration(string name, string returnType, Visibility visibility,
                                 IEnumerable<VariableDeclaration> arguments, LexicalScope body,
                                 IEnumerable<PreconditionStatement> preconditions)
            : this(name, returnType, visibility, arguments, body)
        {
            Preconditions = preconditions;
        }
    }
}