using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Interpreter.Runtime
{
    public class Completion
    {
        public bool ShouldReturn { get; }

        public RuntimeValue Value { get; }

        public Completion(bool shouldReturn, RuntimeValue value)
        {
            ShouldReturn = shouldReturn;
            Value = value;
        }
    }
}