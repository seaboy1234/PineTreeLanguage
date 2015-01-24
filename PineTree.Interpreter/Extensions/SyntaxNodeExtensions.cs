using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter.Extensions
{
    public static class SyntaxNodeExtensions
    {
        public static T As<T>(this SyntaxNode node)
            where T : SyntaxNode
        {
            if (node is T)
            {
                return (T)node;
            }
            return null;
        }
    }
}