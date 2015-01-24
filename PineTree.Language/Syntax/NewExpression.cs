using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class NewExpression : Expression
    {
        public IEnumerable<Expression> Arguments { get; }

        public string Name { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.NewExpression;

        public NewExpression(string name, IEnumerable<Expression> arguments)
        {
            Name = name;
            Arguments = arguments;
        }
    }
}