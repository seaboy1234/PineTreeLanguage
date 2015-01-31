using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Interpreter.Runtime.Environment
{
    public class FunctionCallContext : PineTreeEnvironment
    {
        private PineTreeEngine _engine;
        private Stack<PineTreeEnvironment> _scope;
        private ObjectReference _thisBinding;

        public FunctionCallContext(PineTreeEngine engine, RuntimeObject thisBinding)
        {
            _engine = engine;
            _scope = new Stack<PineTreeEnvironment>();
            _thisBinding = new ObjectReference("this", new RuntimeValue(thisBinding));
        }

        public override PineTreeEnvironment Clone()
        {
            FunctionCallContext context = new FunctionCallContext(_engine, _thisBinding.Value.Value);

            foreach (var scope in _scope)
            {
                context.PushScope(scope);
            }

            foreach (var local in GetReferences())
            {
                context.SetLocal(local.ReferenceName, local.Value);
            }

            return context;
        }

        public override ObjectReference GetReference(string name)
        {
            if (name == "this")
            {
                return _thisBinding;
            }
            if (_scope.Count == 0)
            {
                return base.GetReference(name) ?? _thisBinding?.Value.GetReference(name);
            }
            return base.GetReference(name) ?? _scope.Peek().GetReference(name) ?? _thisBinding?.Value.GetReference(name) ?? _engine.RootContext.GetReference(name);
        }

        public void PopScope()
        {
            _scope.Pop();
        }

        public LexicalEnvironment PushScope()
        {
            PineTreeEnvironment scope = _scope.Count > 0 ? _scope.Peek() : this;
            var env = new LexicalEnvironment(scope);
            PushScope(env);
            return env;
        }

        public override void SetLocal(string name, RuntimeValue value)
        {
            var reference = GetReference(name);
            if (reference != null)
            {
                reference.Value = value;
            }
            else if (_thisBinding.Value.HasProperty(name))
            {
                _thisBinding.Value.SetValue(name, value);
            }
            else
            {
                if (_scope.Count > 0)
                {
                    _scope.Peek().SetLocal(name, value);
                }
                else
                {
                    base.SetLocal(name, value);
                }
            }
        }

        private void PushScope(PineTreeEnvironment scope)
        {
            _scope.Push(scope);
        }
    }
}