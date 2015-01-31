using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Interpreter.Runtime.Environment
{
    public class LexicalEnvironment : PineTreeEnvironment
    {
        private bool _gettingReference;
        private PineTreeEnvironment _parent;

        public LexicalEnvironment(PineTreeEnvironment parent)
        {
            _parent = parent;
        }

        public override PineTreeEnvironment Clone()
        {
            var env = new LexicalEnvironment(_parent.Clone());
            foreach (var reference in GetReferences())
            {
                env.SetLocal(reference.ReferenceName, reference.Value);
            }

            return env;
        }

        public override RuntimeValue GetLocal(string name)
        {
            RuntimeValue value = base.GetLocal(name);
            if (value != RuntimeValue.Null)
            {
                return value;
            }
            return _parent?.GetLocal(name) ?? RuntimeValue.Null;
        }

        public override ObjectReference GetReference(string name)
        {
            if (_gettingReference)
            {
                return base.GetReference(name);
            }
            _gettingReference = true;
            ObjectReference reference = base.GetReference(name) ?? _parent?.GetReference(name);
            _gettingReference = false;
            return reference;
        }
    }
}