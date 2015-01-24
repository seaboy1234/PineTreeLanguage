using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class ArithmeticExpression : BinaryExpression
    {
        public ArithmeticOperator Operation { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.ArithmeticExpression;

        public ArithmeticExpression(Expression left, Expression right, ArithmeticOperator operation)
            : base(left, right)
        {
            Operation = operation;
        }
    }
}