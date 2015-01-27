using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Lexer
{
    /// <summary>
    /// Represents an exception raised from the <see cref="PineTreeLexer"/>.
    /// </summary>
    [Serializable]
    public class LexerException : Exception
    {
        public LexerException()
        {
        }

        public LexerException(string message) : base(message)
        {
        }

        public LexerException(string message, Exception inner) : base(message, inner)
        {
        }

        protected LexerException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}