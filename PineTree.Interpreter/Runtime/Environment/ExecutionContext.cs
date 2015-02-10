using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native;
using PineTree.Interpreter.Native.Function;

namespace PineTree.Interpreter.Runtime.Environment
{
    public class ExecutionContext : PineTreeEnvironment
    {
        private List<Module> _imported;
        private Module _module;

        public ExecutionContext(Module module)
        {
            _module = module;
            _imported = new List<Module>();
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
            ICallable method = null;
            if ((method = _module.FindMethod(name)) != null)
            {
                return new RuntimeValue(method as FunctionInstance);
            }
            else if ((method = GetFromImported(name) as ICallable) != null)
            {
                return new RuntimeValue(method as FunctionInstance);
            }
            return base.GetLocal(name);
        }

        public override ObjectReference GetReference(string name)
        {
            return base.GetReference(name) ?? _module.GetReference(name);
        }

        public override void SetLocal(string name, RuntimeValue value)
        {
            base.SetLocal(name, value);
        }

        internal void AddModule(Module module)
        {
            _imported.Add(module);
        }

        internal TypeMetadata FindType(string name)
        {
            return _module.FindType(name) ?? GetFromImported(g => g.FindType(name));
        }

        private RuntimeObject GetFromImported(string name)
        {
            return GetFromImported(g => g.GetValue(name).Value);
        }

        private T GetFromImported<T>(Func<Module, T> func)
        {
            return _imported.Select(func).FirstOrDefault(g => g != null);
        }
    }
}
