using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native;

namespace PineTree.Interpreter.Runtime
{
    public class PropertyReference
    {
        public ICallable GetMethod { get; }

        public string Name { get; }

        public ICallable SetMethod { get; }

        public PropertyReference(string name, ICallable getMethod, ICallable setMethod)
        {
            Name = name;
            GetMethod = getMethod;
            SetMethod = setMethod;
        }

        public RuntimeValue GetValue(RuntimeValue thisBinding)
        {
            return GetMethod.Invoke(thisBinding, new RuntimeValue[] { });
        }

        public void SetValue(RuntimeValue thisBinding, RuntimeValue value)
        {
            SetMethod?.Invoke(thisBinding, new[] { value });
        }
    }

    public class PropertyValue : ObjectReference
    {
        private readonly PropertyReference _property;
        private readonly RuntimeValue _thisBinding;

        public override RuntimeValue Value
        {
            get { return _property.GetValue(_thisBinding); }
            set { _property.SetValue(_thisBinding, value); }
        }

        public PropertyValue(RuntimeValue thisBinding, PropertyReference reference)
            : base(reference.Name)
        {
            _thisBinding = thisBinding;
            _property = reference;
        }
    }
}