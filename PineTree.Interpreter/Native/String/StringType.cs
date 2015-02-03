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
        public override TypeInfo Info => TypeInfo.String;

        public override string Name => "string";

        public override bool CanCastTo(TypeMetadata typeMetadata)
        {
            return typeMetadata.Info == TypeInfo.String || typeMetadata.Info == TypeInfo.Boolean || typeMetadata.Info == TypeInfo.Int || typeMetadata.Info == TypeInfo.Float;
        }

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