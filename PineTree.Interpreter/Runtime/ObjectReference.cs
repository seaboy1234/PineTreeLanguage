using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Interpreter.Runtime
{
    public class ObjectReference
    {
        public string ReferenceName { get; }

        public virtual RuntimeValue Value { get; set; }

        public ObjectReference(string name)
        {
            ReferenceName = name;
        }

        public ObjectReference(string name, RuntimeValue value)
            : this(name)
        {
            Value = value;
        }
    }
}