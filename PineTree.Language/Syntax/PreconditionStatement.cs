using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class PreconditionStatement : SyntaxNode
    {
        public SyntaxNode Condition { get; }

        public string ErrorMessage { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.PreconditionStatement;

        public PreconditionStatement(SyntaxNode condition, string errorMessage)
        {
            Condition = condition;
            ErrorMessage = errorMessage;
        }
    }
}