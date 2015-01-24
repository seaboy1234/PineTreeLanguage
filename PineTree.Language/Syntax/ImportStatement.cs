using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class ImportStatement : Statement
    {
        public string API { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.ImportStatement;

        public ImportStatement(string api)
        {
            API = api;
        }
    }
}