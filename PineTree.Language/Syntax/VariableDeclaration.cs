using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class VariableDeclaration : Statement
    {
        public string Name { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.VariableDeclaration;

        public string Type { get; }

        public SyntaxNode Value { get; }

        public VariableDeclaration(string name, string type, SyntaxNode value)
        {
            Name = name;
            Type = type;
            Value = value;
        }
    }
}