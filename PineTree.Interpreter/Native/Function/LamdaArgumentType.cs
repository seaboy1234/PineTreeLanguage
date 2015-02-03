using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Native.Function
{
    public class LamdaArgumentType : TypeMetadata
    {
        public override TypeInfo Info => TypeInfo.Unknown;

        public override string Name
        {
            get
            {
                return "Lambda_Argument";
            }
        }

        public override bool CanCastTo(TypeMetadata typeMetadata)
        {
            return true;
        }

        public override RuntimeValue CreateInstance(PineTreeEngine interpreter, RuntimeValue[] args)
        {
            return RuntimeValue.Null;
        }
    }
}