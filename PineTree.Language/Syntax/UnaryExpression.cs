using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class UnaryExpression : Expression
    {
        public bool IsPrefix { get; }

        public UnaryOperator Operation { get; }

        public Expression Parameter { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.UnaryExpression;

        public UnaryExpression(Expression expression, UnaryOperator operation, bool isPrefix)
        {
            Parameter = expression;
            Operation = operation;
            IsPrefix = isPrefix;
        }
    }
}