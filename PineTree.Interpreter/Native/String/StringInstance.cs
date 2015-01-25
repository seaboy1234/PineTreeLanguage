using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Interop;
using PineTree.Interpreter.Native.Integer;
using PineTree.Interpreter.Runtime;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter.Native.String
{
    public class StringInstance : RuntimeObject
    {
        private string _value;

        public override TypeInfo TypeInfo => TypeInfo.String;

        public StringInstance(PineTreeEngine engine)
            : this(engine, string.Empty)
        {
        }

        public StringInstance(PineTreeEngine engine, string value)
            : base(engine)
        {
            _value = value;
            BindMethod("count", new ExternalMethod(engine, this, new Func<int>(() => _value.Length).Method));
        }

        public override RuntimeValue AccessArray(RuntimeValue index)
        {
            IntegerInstance intdex;
            if (!(index.Value is IntegerInstance))
            {
                return RuntimeValue.Null;
            }
            intdex = (IntegerInstance)index.Value;

            return new RuntimeValue(new StringInstance(Engine, _value[(int)intdex.Value].ToString()));
        }

        public override bool Equals(object obj)
        {
            if (obj is StringInstance)
            {
                return ((StringInstance)obj)._value == _value;
            }
            return base.Equals(obj);
        }

        public override RuntimeValue EvaluateArithmetic(RuntimeValue right, ArithmeticOperator op)
        {
            if (right.TypeInfo != TypeInfo.String)
            {
                return RuntimeValue.Null;
            }
            string value = ((StringInstance)right.Value)._value;
            switch (op)
            {
                case ArithmeticOperator.Add:
                    return new RuntimeValue(new StringInstance(Engine, _value + value));

                case ArithmeticOperator.Subtract:
                    return new RuntimeValue(new StringInstance(Engine, _value.Replace(value, string.Empty)));
            }
            return RuntimeValue.Null;
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override object ToClr()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}