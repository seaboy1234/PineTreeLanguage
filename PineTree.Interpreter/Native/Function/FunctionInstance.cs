using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native.Boolean;
using PineTree.Interpreter.Runtime;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter.Native.Function
{
    public delegate RuntimeValue PineTreeFunctionInvoke(RuntimeValue thisBinding, RuntimeValue[] args);

    public class FunctionInstance : RuntimeObject, ICallable
    {
        private readonly MethodDeclaration _methodInfo;
        private string _alias;
        private Lazy<List<TypeMetadata>> _argumentsList;

        public MethodDeclaration Declaration => _methodInfo;

        public string Name => _alias ?? _methodInfo.Name;

        public override TypeInfo TypeInfo => TypeInfo.Function;

        private List<TypeMetadata> _arguments => _argumentsList.Value;

        public FunctionInstance(PineTreeEngine engine, MethodDeclaration method)
            : base(engine)
        {
            _methodInfo = method;
            _argumentsList = new Lazy<List<TypeMetadata>>(() =>
            {
                var list = new List<TypeMetadata>();

                foreach (var arg in method.Arguments)
                {
                    var type = arg.Type;
                    list.Add(engine.FindType(type));
                }

                return list;
            });
        }

        public bool ArgumentsMatch(TypeMetadata[] types)
        {
            return types.SequenceEqual(_arguments);
        }

        public RuntimeValue Invoke(RuntimeValue thisBinding, RuntimeValue[] args)
        {
            Engine.CreateExecutionContext(thisBinding);

            try
            {
                foreach (var arg in _methodInfo.Arguments.Zip(args, (g, f) => new { Name = g.Name, Value = f }))
                {
                    Engine.ExecutionContext.SetLocal(arg.Name, arg.Value);
                }

                if (_methodInfo.Precondition != null)
                {
                    var output = Engine.Evaluate(_methodInfo.Precondition);
                    if (!(output.Value.Value is BooleanInstance))
                    {
                        throw new RuntimeException("Preconditions must return a boolean.");
                    }
                    if (!((bool)output.Value.ToClr()))
                    {
                        throw new RuntimeException("Method Call does not meet preconditions.");
                    }
                }

                var obj = Engine.Evaluate(_methodInfo.Body);

                return obj.Value;
            }
            finally
            {
                Engine.PopExecutionContext();
            }
        }

        public FunctionInstance SetAlias(string alias)
        {
            _alias = alias;
            return this;
        }

        public override object ToClr()
        {
            return new PineTreeFunctionInvoke(Invoke);
        }
    }
}