using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native.Integer;
using PineTree.Interpreter.Native.String;
using PineTree.Interpreter.Runtime;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter.Native.Float
{
    public class FloatInstance : RuntimeObject
    {
        private double _value;

        public override TypeInfo TypeInfo => TypeInfo.Float;

        internal double Value => _value;

        public FloatInstance(PineTreeEngine engine, double value)
            : base(engine)
        {
            _value = value;
        }

        public FloatInstance(PineTreeEngine engine)
            : this(engine, 0)
        {
        }

        public override RuntimeValue EvaluateArithmetic(RuntimeValue right, ArithmeticOperator op)
        {
            if (right.Value is StringInstance)
            {
                string s = ((StringInstance)right.Value).Value;
                switch (op)
                {
                    case ArithmeticOperator.Add:
                        return new RuntimeValue(new StringInstance(Engine, s + _value));

                    case ArithmeticOperator.Subtract:
                        return new RuntimeValue(new StringInstance(Engine, s.Replace(_value.ToString(), string.Empty)));
                }
            }
            else if (right.Value is IntegerInstance)
            {
                long i = ((IntegerInstance)right.Value).Value;
                switch (op)
                {
                    case ArithmeticOperator.Add:
                        return new RuntimeValue(new FloatInstance(Engine, _value + i));

                    case ArithmeticOperator.Subtract:
                        return new RuntimeValue(new FloatInstance(Engine, _value - i));

                    case ArithmeticOperator.Divide:
                        return new RuntimeValue(new FloatInstance(Engine, _value / i));

                    case ArithmeticOperator.Multiply:
                        return new RuntimeValue(new FloatInstance(Engine, _value * i));
                }
            }
            switch (op)
            {
                case ArithmeticOperator.Add:
                    return new RuntimeValue(new FloatInstance(Engine, _value + right.As<FloatInstance>()._value));

                case ArithmeticOperator.Subtract:
                    return new RuntimeValue(new FloatInstance(Engine, _value - right.As<FloatInstance>()._value));

                case ArithmeticOperator.Multiply:
                    return new RuntimeValue(new FloatInstance(Engine, _value * right.As<FloatInstance>()._value));

                case ArithmeticOperator.Divide:
                    return new RuntimeValue(new FloatInstance(Engine, _value / right.As<FloatInstance>()._value));

                case ArithmeticOperator.Modulo:
                    return new RuntimeValue(new FloatInstance(Engine, _value % right.As<FloatInstance>()._value));

                default:
                    return RuntimeValue.Null;
            }
        }

        public override RuntimeValue EvaluateUnaryExpression(bool isPrefix, UnaryOperator operation)
        {
            if (isPrefix)
            {
                if (operation == UnaryOperator.Decrement)
                {
                    return new RuntimeValue(new FloatInstance(Engine, --_value));
                }
                else if (operation == UnaryOperator.Increment)
                {
                    return new RuntimeValue(new FloatInstance(Engine, ++_value));
                }
                else if (operation == UnaryOperator.Negation)
                {
                    return new RuntimeValue(new FloatInstance(Engine, -_value));
                }
            }
            else
            {
                if (operation == UnaryOperator.Decrement)
                {
                    return new RuntimeValue(new FloatInstance(Engine, _value--));
                }
                else if (operation == UnaryOperator.Increment)
                {
                    return new RuntimeValue(new FloatInstance(Engine, _value++));
                }
            }

            return RuntimeValue.Null;
        }

        public TypeMetadata GetTypeMetadata(PineTreeEngine engine)
        {
            return engine.FindType("float");
        }

        public override object ToClr()
        {
            return _value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}