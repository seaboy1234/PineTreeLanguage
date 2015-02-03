using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Native.Error
{
    public class ErrorType : TypeMetadata
    {
        public override TypeInfo Info => TypeInfo.Error;

        public override string Name => "Error";

        public override bool CanCastTo(TypeMetadata typeMetadata)
        {
            return typeMetadata.Info == TypeInfo.Error;
        }

        public override RuntimeValue CreateInstance(PineTreeEngine interpreter, RuntimeValue[] args)
        {
            if (args.Length == 1)
            {
                return new RuntimeValue(new ErrorInstance(interpreter, args[0].ToString()));
            }
            return new RuntimeValue(new ErrorInstance(interpreter, args.FirstOrDefault().Value?.ToString() ?? string.Empty));
        }
    }
}