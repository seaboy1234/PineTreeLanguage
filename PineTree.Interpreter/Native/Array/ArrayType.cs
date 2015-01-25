using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Native.Array
{
    public class ArrayType : TypeMetadata
    {
        public override string Name => "array";

        public override RuntimeValue CreateInstance(PineTreeEngine interpreter, RuntimeValue[] args)
        {
            return RuntimeValue.Null;
        }
    }
}