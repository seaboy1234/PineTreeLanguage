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
        public override TypeInfo Info => TypeInfo.Array;

        public override string Name => "array";

        public override bool CanCastTo(TypeMetadata typeMetadata)
        {
            return typeMetadata.Info == TypeInfo.Array;
        }

        public override RuntimeValue CreateInstance(PineTreeEngine interpreter, RuntimeValue[] args)
        {
            return RuntimeValue.Null;
        }
    }
}