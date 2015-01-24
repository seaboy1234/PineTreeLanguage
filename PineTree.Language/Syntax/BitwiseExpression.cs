using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class BitwiseExpression : BinaryExpression
    {
        public BitwiseOperator Operation { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.BitwiseExpression;

        public BitwiseExpression(Expression left, Expression right, BitwiseOperator operation)
            : base(left, right)
        {
            Operation = operation;
        }
    }
}