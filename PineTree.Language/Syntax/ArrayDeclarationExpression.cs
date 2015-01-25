using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class ArrayDeclarationExpression : Expression
    {
        public Expression Size { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.ArrayDeclarationExpression;

        public string Type { get; }

        public ArrayDeclarationExpression(string type, Expression size)
        {
            Type = type;
            Size = size;
        }
    }
}