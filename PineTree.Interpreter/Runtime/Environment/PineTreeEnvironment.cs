using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Interpreter.Runtime.Environment
{
    public abstract class PineTreeEnvironment
    {
        private List<ObjectReference> _locals;

        public PineTreeEnvironment()
        {
            _locals = new List<ObjectReference>();
        }

        public abstract PineTreeEnvironment Clone();

        public virtual RuntimeValue GetLocal(string name)
        {
            return GetReference(name)?.Value ?? RuntimeValue.Null;
        }

        public virtual ObjectReference GetReference(string name)
        {
            return _locals.Find(g => g.ReferenceName == name);
        }

        public IEnumerable<ObjectReference> GetReferences()
        {
            return _locals.AsReadOnly();
        }

        public virtual bool IsDefined(string name)
        {
            return GetReference(name) != null;
        }

        public virtual void SetLocal(string name, RuntimeValue value)
        {
            var obj = _locals.Find(g => g.ReferenceName == name);
            if (obj != null)
            {
                obj.Value = value;
            }
            else
            {
                _locals.Add(new ObjectReference(name, value));
            }
        }
    }
}