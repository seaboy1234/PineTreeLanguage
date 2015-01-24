using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class LexicalScope : SyntaxNode
    {
        public IEnumerable<SyntaxNode> Statements { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.LexicalScope;

        public LexicalScope(IEnumerable<SyntaxNode> statements)
        {
            Statements = statements;
        }
    }
}