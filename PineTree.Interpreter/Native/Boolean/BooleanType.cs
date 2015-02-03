using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native.Float;
using PineTree.Interpreter.Native.Integer;
using PineTree.Interpreter.Native.String;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Native.Boolean
{
    public class BooleanType : TypeMetadata
    {
        public override TypeInfo Info => TypeInfo.Boolean;

        public override string Name => "boolean";

        public override bool CanCastTo(TypeMetadata typeMetadata)
        {
            return typeMetadata.Info == TypeInfo.Boolean || typeMetadata.Info == TypeInfo.String;
        }

        public override RuntimeValue CreateInstance(PineTreeEngine engine, RuntimeValue[] args)
        {
            if (args.Length >= 1)
            {
                switch (args[0].Value?.TypeInfo)
                {
                    case TypeInfo.Boolean:
                        return args[0];

                    case TypeInfo.Float:
                        return new RuntimeValue(new BooleanInstance(engine, args[0].As<FloatInstance>().Equals(new FloatInstance(engine, 1))));

                    case TypeInfo.String:
                        return new RuntimeValue(new BooleanInstance(engine, args[0].As<StringInstance>().Equals(new StringInstance(engine, "true"))));

                    case TypeInfo.Int:
                        return new RuntimeValue(new BooleanInstance(engine, args[0].As<IntegerInstance>().Equals(new IntegerInstance(engine, 1))));
                }
            }
            return new RuntimeValue(new BooleanInstance(engine, false));
        }
    }
}