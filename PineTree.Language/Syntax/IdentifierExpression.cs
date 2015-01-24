using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class IdentifierExpression : ExpressionTerminal
    {
        public string Identifier { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.IdentifierExpression;

        public IdentifierExpression(string identifier)
        {
            Identifier = identifier;
        }
    }
}