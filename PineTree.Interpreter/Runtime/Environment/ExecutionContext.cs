using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Interpreter.Runtime.Environment
{
    public class ExecutionContext : PineTreeEnvironment
    {
        private Module _module;

        public ExecutionContext(Module module)
        {
            _module = module;
        }

        public override PineTreeEnvironment Clone()
        {
            var context = new ExecutionContext(_module);
            foreach (var reference in GetReferences())
            {
                context.SetLocal(reference.ReferenceName, reference.Value);
            }

            return context;
        }

        public override RuntimeValue GetLocal(string name)
        {
            return base.GetLocal(name);
        }

        public override void SetLocal(string name, RuntimeValue value)
        {
            base.SetLocal(name, value);
        }
    }
}