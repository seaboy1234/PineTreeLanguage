using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Interpreter.Runtime.Environment
{
    public class LexicalEnvrionment : PineTreeEnvironment
    {
        private PineTreeEnvironment _parent;

        public LexicalEnvrionment(PineTreeEnvironment parent)
        {
            _parent = parent;
        }

        public override PineTreeEnvironment Clone()
        {
            var env = new LexicalEnvrionment(_parent.Clone());
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
    }
}