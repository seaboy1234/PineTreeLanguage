using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Interop
{
    public class ExternalMethod : RuntimeObject, ICallable
    {
        private DynamicMethod _dynamicMethod;
        private PineTreeEngine _engine;
        private MethodInfo _method;
        private ParameterInfo[] _parameters;
        private object _target;

        public override Runtime.TypeInfo TypeInfo => Runtime.TypeInfo.Function;

        public ExternalMethod(PineTreeEngine engine, object target, MethodInfo method)
                    : base(engine)
        {
            _engine = engine;
            _method = method;
            _target = target;

            _parameters = method.GetParameters();

            _dynamicMethod = new DynamicMethod(method.Name, method.ReturnType, _parameters.Select(g => g.ParameterType).ToArray());
        }

        public ExternalMethod(PineTreeEngine engine, Delegate target)
            : this(engine, target.Target, target.Method)
        {
        }

        public bool ArgumentsMatch(TypeMetadata[] types)
        {
            return true;
        }

        public RuntimeValue Invoke(RuntimeValue thisBinding, RuntimeValue[] args)
        {
            var obj = _method.Invoke(_target, ConvertArgs(args));

            if (obj == null)
            {
                return RuntimeValue.Null;
            }

            return _engine.CastObject(obj);
        }

        public override object ToClr()
        {
            return new MethodReference(_target, _method);
        }

        public override string ToString()
        {
            return "function";
        }

        private object[] ConvertArgs(RuntimeValue[] args)
        {
            object[] outArgs = new object[_parameters.Length];
            if (_parameters.Length == 0)
            {
                return outArgs;
            }
            for (int i = 0; i < _parameters.Length; i++)
            {
                if (_parameters[i].ParameterType == typeof(RuntimeValue))
                {
                    outArgs[i] = args[i];
                    continue;
                }
                outArgs[i] = args[i].ToClr();
            }

            return outArgs;
        }
    }

    public class MethodReference
    {
        private MethodInfo _method;
        private object _target;

        public MethodReference(object target, MethodInfo method)
        {
            _target = target;
            _method = method;
        }

        public object Invoke()
        {
            return _method.Invoke(_target, null);
        }

        public object Invoke(object[] args)
        {
            return _method.Invoke(_target, args);
        }

        public T1 Invoke<T1>()
        {
            return (T1)Invoke();
        }

        public T1 Invoke<T1, T2>(T2 arg)
        {
            return (T1)Invoke(new object[] { arg });
        }

        public T1 Invoke<T1, T2, T3>(T2 arg1, T3 arg2)
        {
            return (T1)Invoke(new object[] { arg1, arg2 });
        }

        public T1 Invoke<T1, T2, T3, T4>(T2 arg1, T3 arg2, T4 arg3)
        {
            return (T1)Invoke(new object[] { arg1, arg2, arg3 });
        }

        public T1 Invoke<T1, T2, T3, T4, T5>(T2 arg1, T3 arg2, T4 arg3, T5 arg4)
        {
            return (T1)Invoke(new object[] { arg1, arg2, arg3, arg4 });
        }

        public T1 Invoke<T1, T2, T3, T4, T5, T6>(T2 arg1, T3 arg2, T4 arg3, T5 arg4, T6 arg5)
        {
            return (T1)Invoke(new object[] { arg1, arg2, arg3, arg4, arg5 });
        }
    }
}