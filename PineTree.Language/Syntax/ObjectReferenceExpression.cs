using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class ObjectReferenceExpression : Expression
    {
        public Expression Final => References.Last();

        public IEnumerable<Expression> References { get; }

        public Expression Root { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.ObjectReferenceExpression;

        public ObjectReferenceExpression(Expression root, IEnumerable<Expression> references)
        {
            Root = root;
            References = references;
        }
    }
}