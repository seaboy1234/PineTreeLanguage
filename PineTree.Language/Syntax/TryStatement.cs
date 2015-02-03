using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class TryStatement : Statement
    {
        public SyntaxNode Body { get; }

        public CatchStatement CatchStatement { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.TryStatement;

        public TryStatement(SyntaxNode body, CatchStatement catchStatement)
        {
            Body = body;
            CatchStatement = catchStatement;
        }
    }
}