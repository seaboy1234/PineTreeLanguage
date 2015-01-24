using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Interpreter.Runtime
{
    public abstract class TypeMetadata
    {
        public abstract string Name { get; }

        public abstract RuntimeValue CreateInstance(PineTreeEngine interpreter, RuntimeValue[] args);
    }
}