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
        public override TypeInfo Info => TypeInfo.Int;

        public override string Name => "int";

        public override bool CanCastTo(TypeMetadata typeMetadata)
        {
            return typeMetadata.Info == TypeInfo.Int || typeMetadata.Info == TypeInfo.String || typeMetadata.Info == TypeInfo.Float || typeMetadata.Info == TypeInfo.Boolean;
        }

        public override RuntimeValue CreateInstance(PineTreeEngine interpreter, RuntimeValue[] args)
        {
            if (args.Length == 1)
            {
                switch (args[0].TypeInfo)
                {
                    case TypeInfo.Int:
                        return args[0];

                    case TypeInfo.Null:
                        return new RuntimeValue(new IntegerInstance(interpreter));

                    case TypeInfo.String:
                        return new RuntimeValue(new IntegerInstance(interpreter, int.Parse(args[0].ToString())));

                    case TypeInfo.Float:
                        return new RuntimeValue(new IntegerInstance(interpreter, (int)((double)args[0].ToClr())));
                }
            }
            return RuntimeValue.Null;
        }
    }
}