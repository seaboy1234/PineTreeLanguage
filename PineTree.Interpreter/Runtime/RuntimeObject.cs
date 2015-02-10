using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Interop;
using PineTree.Interpreter.Native;
using PineTree.Interpreter.Native.Boolean;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter.Runtime
{
    public abstract class RuntimeObject
    {
        private List<MethodReference> _methods;
        private List<ObjectReference> _properties;

        public PineTreeEngine Engine { get; }

        public abstract TypeInfo TypeInfo { get; }

        public virtual string TypeName => TypeInfo.ToString().ToLower();

        private class MethodReference : ICallable
        {
            public ICallable Method { get; }

            public string Name { get; }

            public MethodReference(string name, ICallable method)
            {
                Name = name;
                Method = method;
            }

            public bool ArgumentsMatch(TypeMetadata[] types)
            {
                return Method?.ArgumentsMatch(types) ?? false;
            }

            public RuntimeValue Invoke(RuntimeValue thisBinding, RuntimeValue[] args)
            {
                return Method?.Invoke(thisBinding, args) ?? RuntimeValue.Null;
            }
        }

        protected RuntimeObject(PineTreeEngine engine)
        {
            _methods = new List<MethodReference>();
            _properties = new List<ObjectReference>();
            Engine = engine;
        }

        public virtual RuntimeValue AccessArray(RuntimeValue index)
        {
            return RuntimeValue.Null;
        }

        public void BindField(string name, RuntimeValue value)
        {
            _properties.Add(new ObjectReference(name, value));
        }

        public void BindField(ObjectReference reference)
        {
            _properties.Add(reference);
        }

        public void BindMethod(string name, ICallable method)
        {
            _methods.Add(new MethodReference(name, method));
        }

        public void BindMethod(string name, Delegate method)
        {
            BindMethod(name, new ExternalMethod(Engine, method));
        }

        public void BindProperty(PropertyReference property)
        {
            _properties.Add(new PropertyValue(new RuntimeValue(this), property));
        }

        public void BindProperty(string name, ICallable getMethod)
        {
            BindProperty(name, getMethod, null);
        }

        public void BindProperty(string name, ICallable getMethod, ICallable setMethod)
        {
            BindProperty(new PropertyReference(name, getMethod, setMethod));
        }

        public virtual RuntimeValue EvaluateArithmetic(RuntimeValue right, ArithmeticOperator op)
        {
            return RuntimeValue.Null;
        }

        public virtual RuntimeValue EvaluateLogicalOp(RuntimeValue right, LogicOperation op)
        {
            switch (op)
            {
                case LogicOperation.Equals:
                    return new RuntimeValue(new BooleanInstance(Engine, Equals(right.Value)));

                case LogicOperation.NotEquals:
                    return new RuntimeValue(new BooleanInstance(Engine, !Equals(right.Value)));
            }
            return RuntimeValue.Null;
        }

        public virtual RuntimeValue EvaluateUnaryExpression(bool isPrefix, UnaryOperator operation)
        {
            return RuntimeValue.Null;
        }

        public bool HasProperty(string name)
        {
            return _properties.Find(g => g.ReferenceName == name) != null;
        }

        public virtual ICallable ResolveMethod(string name, TypeMetadata[] types)
        {
            return _methods.Find(g => g.Name == name && g.ArgumentsMatch(types));
        }

        public virtual void SetArrayValue(RuntimeValue index, RuntimeValue value)
        {
        }

        public abstract object ToClr();

        internal ICallable FindMethod(string name)
        {
            return _methods.Find(g => g.Name == name);
        }

        internal ICallable FindMethod(string name, TypeMetadata[] types)
        {
            return _methods.Find(g => g.Name == name && g.ArgumentsMatch(types));
        }

        internal ObjectReference GetReference(string name)
        {
            return _properties.Find(g => g.ReferenceName == name);
        }

        internal RuntimeValue GetValue(string identifier)
        {
            var property = _properties.Find(g => g.ReferenceName == identifier);
            if (property != null)
            {
                return property.Value;
            }
            return new RuntimeValue(_methods.Find(g => g.Name == identifier)?.Method as RuntimeObject);
        }

        internal void SetValue(string identifier, RuntimeValue value)
        {
            var property = _properties.Find(g => g.ReferenceName == identifier);
            if (property != null)
            {
                property.Value = value;
            }
        }
    }
}
