using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Interpreter.Runtime
{
    public enum TypeInfo
    {
        Null,
        Unknown,

        String,
        Int,
        Float,
        Boolean,

        Function,
        Object,
        Array,
    }
}