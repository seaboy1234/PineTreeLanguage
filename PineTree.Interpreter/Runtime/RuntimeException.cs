using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native.Error;

namespace PineTree.Interpreter.Runtime
{
    [Serializable]
    public class RuntimeException : Exception
    {
        public ErrorInstance Error { get; }

        public RuntimeException()
        {
        }

        public RuntimeException(string message) : base(message)
        {
        }

        public RuntimeException(string message, ErrorInstance errorInstance) : this(message)
        {
            Error = errorInstance;
        }

        public RuntimeException(string message, Exception inner) : base(message, inner)
        {
        }

        protected RuntimeException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context)
        { }
    }
}