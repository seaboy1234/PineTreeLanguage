﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Native.Float
{
    public class FloatType : TypeMetadata
    {
        public override string Name => "float";

        public override RuntimeValue CreateInstance(PineTreeEngine interpreter, RuntimeValue[] args)
        {
            double value = 0f;
            if (args.Length > 0)
            {
                if (args[0].TypeInfo == TypeInfo.Float)
                {
                    return args[0];
                }
                double.TryParse(args[0].ToString(), out value);
            }
            return new RuntimeValue(new FloatInstance(interpreter, value));
        }
    }
}