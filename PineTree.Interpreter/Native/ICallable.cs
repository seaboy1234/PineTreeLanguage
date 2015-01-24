using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Native
{
    public interface ICallable
    {
        bool ArgumentsMatch(TypeMetadata[] types);

        RuntimeValue Invoke(RuntimeValue thisBinding, RuntimeValue[] args);
    }
}