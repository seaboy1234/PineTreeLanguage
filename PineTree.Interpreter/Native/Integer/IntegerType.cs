using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Native.Integer
{
    public class IntegerType : TypeMetadata
    {
        public override string Name => "int";

        public override RuntimeValue CreateInstance(PineTreeEngine interpreter, RuntimeValue[] args)
        {
            int value = 0;
            if (args.Length > 0)
            {
                int.TryParse(args[0].ToString(), out value);
            }
            return new RuntimeValue(new IntegerInstance(interpreter, value));
        }
    }
}