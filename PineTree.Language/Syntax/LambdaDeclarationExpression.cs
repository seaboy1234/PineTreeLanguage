using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class LambdaExpression : Expression
    {
        public IEnumerable<VariableDeclaration> Arguments { get; }

        public SyntaxNode Body { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.LambdaExpression;

        public LambdaExpression(IEnumerable<VariableDeclaration> arguments, SyntaxNode body)
        {
            Arguments = arguments;
            Body = body;
        }
    }
}