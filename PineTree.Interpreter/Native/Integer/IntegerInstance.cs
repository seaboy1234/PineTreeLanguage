using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native.Boolean;
using PineTree.Interpreter.Native.Float;
using PineTree.Interpreter.Native.String;
using PineTree.Interpreter.Runtime;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter.Native.Integer
{
    public class IntegerInstance : RuntimeObject
    {
        private long _value;

        public override TypeInfo TypeInfo => TypeInfo.Int;

        public long Value => _value;

        public IntegerInstance(PineTreeEngine engine, long value)
            : base(engine)
        {
            _value = value;
        }

        public IntegerInstance(PineTreeEngine engine)
            : this(engine, 0)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj is IntegerInstance)
            {
                return ((IntegerInstance)obj)._value == _value;
            }
            return base.Equals(obj);
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

                    case ArithmeticOperator.Multiply:
                        for (int i = 0; i < _value; i++)
                        {
                            s += s;
                        }
                        return new RuntimeValue(new StringInstance(Engine, s));
                }
            }
            else if (right.Value is FloatInstance)
            {
                double d = ((FloatInstance)right.Value).Value;
                switch (op)
                {
                    case ArithmeticOperator.Add:
                        return new RuntimeValue(new FloatInstance(Engine, _value + d));

                    case ArithmeticOperator.Subtract:
                        return new RuntimeValue(new FloatInstance(Engine, _value - d));

                    case ArithmeticOperator.Divide:
                        return new RuntimeValue(new FloatInstance(Engine, _value / d));

                    case ArithmeticOperator.Multiply:
                        return new RuntimeValue(new FloatInstance(Engine, _value * d));
                }
            }

            switch (op)
            {
                case ArithmeticOperator.Add:
                    return new RuntimeValue(new IntegerInstance(Engine, _value + right.As<IntegerInstance>()._value));

                case ArithmeticOperator.Subtract:
                    return new RuntimeValue(new IntegerInstance(Engine, _value - right.As<IntegerInstance>()._value));

                case ArithmeticOperator.Multiply:
                    return new RuntimeValue(new IntegerInstance(Engine, _value * right.As<IntegerInstance>()._value));

                case ArithmeticOperator.Divide:
                    return new RuntimeValue(new IntegerInstance(Engine, _value / right.As<IntegerInstance>()._value));

                case ArithmeticOperator.Modulo:
                    return new RuntimeValue(new IntegerInstance(Engine, _value % right.As<IntegerInstance>()._value));

                default:
                    return RuntimeValue.Null;
            }
        }

        public override RuntimeValue EvaluateLogicalOp(RuntimeValue right, LogicOperation op)
        {
            var value = base.EvaluateLogicalOp(right, op);
            if (value != RuntimeValue.Null)
            {
                return value;
            }

            if (!(right.Value is IntegerInstance))
            {
                return RuntimeValue.Null;
            }

            long compare = ((IntegerInstance)right.Value)._value;

            switch (op)
            {
                case LogicOperation.GreaterThan:
                    return new RuntimeValue(new BooleanInstance(Engine, _value > compare));

                case LogicOperation.GreaterThanOrEqual:
                    return new RuntimeValue(new BooleanInstance(Engine, _value >= compare));

                case LogicOperation.LessThan:
                    return new RuntimeValue(new BooleanInstance(Engine, _value < compare));

                case LogicOperation.LessThanOrEqual:
                    return new RuntimeValue(new BooleanInstance(Engine, _value <= compare));
            }

            return value;
        }

        public override RuntimeValue EvaluateUnaryExpression(bool isPrefix, UnaryOperator operation)
        {
            if (isPrefix)
            {
                if (operation == UnaryOperator.Decrement)
                {
                    return new RuntimeValue(new IntegerInstance(Engine, --_value));
                }
                else if (operation == UnaryOperator.Increment)
                {
                    return new RuntimeValue(new IntegerInstance(Engine, ++_value));
                }
                else if (operation == UnaryOperator.Negation)
                {
                    return new RuntimeValue(new IntegerInstance(Engine, -_value));
                }
            }
            else
            {
                if (operation == UnaryOperator.Decrement)
                {
                    return new RuntimeValue(new IntegerInstance(Engine, _value--));
                }
                else if (operation == UnaryOperator.Increment)
                {
                    return new RuntimeValue(new IntegerInstance(Engine, _value++));
                }
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
            return _value.ToString();
        }
    }
}