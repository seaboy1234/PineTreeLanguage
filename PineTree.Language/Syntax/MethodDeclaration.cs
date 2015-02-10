using System.Collections;
using System.Collections.Generic;

namespace PineTree.Language.Syntax
{
	public class MethodDeclaration : SyntaxNode
	{
		public IEnumerable<VariableDeclaration> Arguments { get; }

		public LexicalScope Body { get; }

		public bool IsStatic { get; }

		public string Name { get; }

		public IEnumerable<PreconditionStatement> Preconditions { get; }

		public string ReturnType { get; }

		public override SyntaxTypes SyntaxType => SyntaxTypes.MethodDeclaration;

		public Visibility Visibility { get; }

		public MethodDeclaration(string name, string returnType, bool isStatic, Visibility visibility, IEnumerable<VariableDeclaration> arguments, LexicalScope body)
		{
			Name = name;
			ReturnType = returnType;
			IsStatic = isStatic;
			Visibility = visibility;
			Arguments = arguments;
			Body = body;
		}

		public MethodDeclaration(string name, string returnType, bool isStatic, Visibility visibility,
								 IEnumerable<VariableDeclaration> arguments, LexicalScope body,
								 IEnumerable<PreconditionStatement> preconditions)
			: this(name, returnType, isStatic, visibility, arguments, body)
		{
			Preconditions = preconditions;
		}
	}
}