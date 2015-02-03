using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Interpreter.Runtime
{
    public abstract class TypeMetadata
    {
        internal static readonly TypeMetadata[] Empty = new TypeMetadata[0];

        public abstract TypeInfo Info { get; }

        public abstract string Name { get; }

        public abstract bool CanCastTo(TypeMetadata typeMetadata);

        public abstract RuntimeValue CreateInstance(PineTreeEngine interpreter, RuntimeValue[] args);
    }
}