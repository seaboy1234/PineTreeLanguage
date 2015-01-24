using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class SourceDocument : SyntaxNode
    {
        public IEnumerable<SyntaxNode> Contents { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.SourceDocument;

        public SourceDocument(IEnumerable<SyntaxNode> contents)
        {
            Contents = contents;
        }
    }
}