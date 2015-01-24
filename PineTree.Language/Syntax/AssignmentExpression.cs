using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class AssignmentExpression : BinaryExpression
    {
        public AssignmentType Assignment { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.AssignmentExpression;

        public AssignmentExpression(Expression left, Expression right, AssignmentType assignment)
            : base(left, right)
        {
            Assignment = assignment;
        }
    }
}