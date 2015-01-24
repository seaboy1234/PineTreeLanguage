using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Native.String
{
    public class StringType : TypeMetadata
    {
        public override string Name => "string";

        public override RuntimeValue CreateInstance(PineTreeEngine interpreter, RuntimeValue[] args)
        {
            string value = string.Empty;
            if (args.Length > 0)
            {
                value = args[0].ToString();
            }
            return new RuntimeValue(new StringInstance(interpreter, value));
        }
    }
}