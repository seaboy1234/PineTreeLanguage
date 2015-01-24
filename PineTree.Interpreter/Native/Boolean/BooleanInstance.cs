using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Runtime;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter.Native.Boolean
{
    public class BooleanInstance : RuntimeObject
    {
        private bool _value;

        public override TypeInfo TypeInfo => TypeInfo.Boolean;

        public BooleanInstance(PineTreeEngine engine, bool value)
            : base(engine)
        {
            _value = value;
        }

        public BooleanInstance(PineTreeEngine engine)
            : this(engine, false)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj is BooleanInstance)
            {
                return _value == ((BooleanInstance)obj)._value;
            }

            return base.Equals(obj);
        }

        public override RuntimeValue EvaluateLogicalOp(RuntimeValue right, LogicOperation op)
        {
            var value = base.EvaluateLogicalOp(right, op);
            if (value != RuntimeValue.Null)
            {
                return value;
            }
            if (!(right.Value is BooleanInstance))
            {
                return value;
            }

            bool compare = ((BooleanInstance)right.Value)._value;

            switch (op)
            {
                case LogicOperation.And:
                    return new RuntimeValue(new BooleanInstance(Engine, _value && compare));

                case LogicOperation.Or:
                    return new RuntimeValue(new BooleanInstance(Engine, _value || compare));
            }

            return value;
        }

        public override RuntimeValue EvaluateUnaryExpression(bool isPrefix, UnaryOperator operation)
        {
            if (operation == UnaryOperator.LogicalNot)
            {
                return new RuntimeValue(new BooleanInstance(Engine, !_value));
            }
            return base.EvaluateUnaryExpression(isPrefix, operation);
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