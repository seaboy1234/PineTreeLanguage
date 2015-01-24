using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class MethodCallExpression : Expression
    {
        public IEnumerable<Expression> Arguments { get; }

        public string Name { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.MethodCallExpression;

        public MethodCallExpression(string name, IEnumerable<Expression> arguments)
        {
            Name = name;
            Arguments = arguments;
        }
    }
}