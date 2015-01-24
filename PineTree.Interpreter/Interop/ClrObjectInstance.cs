using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Runtime;
using TypeInfo = PineTree.Interpreter.Runtime.TypeInfo;

namespace PineTree.Interpreter.Interop
{
    public class ClrObjectInstance : RuntimeObject
    {
        private object _target;
        private Type _type;

        public override TypeInfo TypeInfo => TypeInfo.Object;

        public override string TypeName => _type.Name;

        private class ClrField : ObjectReference
        {
            private PineTreeEngine _engine;
            private FieldInfo _field;
            private object _target;

            public override RuntimeValue Value
            {
                get
                {
                    return _engine.CastObject(_field.GetValue(_target));
                }
                set
                {
                    _field.SetValue(_target, value.ToClr());
                }
            }

            public ClrField(FieldInfo field, ClrObjectInstance obj)
                : base(field.Name)
            {
                _engine = obj.Engine;
                _target = obj._target;
                _field = field;
            }
        }

        private class ClrProperty : ObjectReference
        {
            private PineTreeEngine _engine;
            private PropertyInfo _property;
            private object _target;

            public override RuntimeValue Value
            {
                get
                {
                    return _engine.CastObject(_property.GetValue(_target));
                }
                set
                {
                    _property.SetValue(_target, value.ToClr());
                }
            }

            public ClrProperty(PropertyInfo property, ClrObjectInstance obj)
                : base(property.Name)
            {
                _engine = obj.Engine;
                _target = obj._target;
                _property = property;
            }
        }

        public ClrObjectInstance(PineTreeEngine engine, object target)
            : base(engine)
        {
            _target = target;
            _type = target.GetType();

            foreach (var property in _type.GetProperties())
            {
                BindField(new ClrProperty(property, this));
            }
            foreach (var field in _type.GetFields())
            {
                BindField(new ClrField(field, this));
            }

            foreach (var method in _type.GetMethods())
            {
                BindMethod(method.Name, new ExternalMethod(Engine, _target, method));
            }
        }

        public override object ToClr()
        {
            return _target;
        }
    }
}