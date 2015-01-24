using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class LogicalExpression : BinaryExpression
    {
        public LogicOperation Operation { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.LogicalExpression;

        public LogicalExpression(Expression left, Expression right, LogicOperation operation)
            : base(left, right)
        {
            Operation = operation;
        }
    }
}