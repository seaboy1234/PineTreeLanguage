using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native.Boolean;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter.Runtime
{
    public struct RuntimeValue : IEquatable<RuntimeValue>
    {
        public static readonly RuntimeValue Null = new RuntimeValue();

        public RuntimeObject Value;

        public TypeInfo TypeInfo => Value?.TypeInfo ?? TypeInfo.Null;

        public string TypeName => Value?.TypeName ?? "null";

        public RuntimeValue(RuntimeObject value)
        {
            Value = value;
        }

        public static bool operator !=(RuntimeValue left, RuntimeValue right)
        {
            return !(left == right);
        }

        public static bool operator ==(RuntimeValue left, RuntimeValue right)
        {
            return left.Value?.Equals(right.Value) ?? right.Value == null;
        }

        public T As<T>()
            where T : RuntimeObject
        {
            return Value as T;
        }

        public override bool Equals(object obj)
        {
            if (obj is RuntimeValue)
            {
                return Equals((RuntimeValue)obj);
            }
            return base.Equals(obj);
        }

        public bool Equals(RuntimeValue other)
        {
            return this == other;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? base.GetHashCode();
        }

        public ObjectReference GetReference(string name)
        {
            return Value?.GetReference(name);
        }

        public bool IsNull()
        {
            return this == Null;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "null";
        }

        internal RuntimeValue AccessArrayIndex(RuntimeValue value)
        {
            return Value?.AccessArray(value) ?? Null;
        }

        internal RuntimeValue EvaluateArithmetic(RuntimeValue right, ArithmeticOperator operation)
        {
            return Value?.EvaluateArithmetic(right, operation) ?? Null;
        }

        internal RuntimeValue EvaluateLogic(RuntimeValue right, LogicOperation operation)
        {
            return Value?.EvaluateLogicalOp(right, operation) ?? new RuntimeValue(new BooleanInstance(right.Value?.Engine, false));
        }

        internal RuntimeValue EvaluateUnaryExpression(bool isPrefix, UnaryOperator operation)
        {
            return Value?.EvaluateUnaryExpression(isPrefix, operation) ?? Null;
        }

        internal RuntimeValue GetValue(string identifier)
        {
            return Value?.GetValue(identifier) ?? Null;
        }

        internal bool HasProperty(string name)
        {
            return Value?.HasProperty(name) ?? false;
        }

        internal void SetArrayIndex(RuntimeValue index, RuntimeValue value)
        {
            Value?.SetArrayValue(index, value);
        }

        internal void SetValue(string identifier, RuntimeValue value)
        {
            Value?.SetValue(identifier, value);
        }

        internal object ToClr()
        {
            if (TypeInfo == TypeInfo.Null)
            {
                return null;
            }

            return Value.ToClr();
        }
    }
}