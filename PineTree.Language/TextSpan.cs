using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language
{
    /// <summary>
    /// Represents a span of text in a source file.
    /// </summary>
    public struct TextSpan
    {
        public int Length;
        public int Lines;

        public TextSpan(Position start, Position end)
        {
            Length = end.FileIndex - start.FileIndex;
            Lines = end.LineNumber - start.LineNumber;
        }

        public override string ToString()
        {
            return $"{Lines} {Length}";
        }
    }
}