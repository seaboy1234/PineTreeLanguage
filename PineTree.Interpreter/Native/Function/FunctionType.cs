﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Native.Function
{
    public class FunctionType : TypeMetadata
    {
        public override TypeInfo Info => TypeInfo.Function;

        public override string Name => "function";

        public override bool CanCastTo(TypeMetadata typeMetadata)
        {
            return typeMetadata.Info == TypeInfo.Function;
        }

        public override RuntimeValue CreateInstance(PineTreeEngine interpreter, RuntimeValue[] args)
        {
            if (args.Length != 2)
            {
                return RuntimeValue.Null;
            }
            var name = args[0];
            var func = args[1];
            if (name.TypeInfo != TypeInfo.String || func.TypeInfo != TypeInfo.Function)
            {
                return RuntimeValue.Null;
            }
            if (func.Value is LambdaInstance)
            {
                return func;
            }

            return new RuntimeValue(new FunctionInstance(interpreter, func.As<FunctionInstance>().Declaration).SetAlias(name.ToString()));
        }
    }
}