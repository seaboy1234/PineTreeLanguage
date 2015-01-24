using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Runtime;
using PineTree.Interpreter.Runtime.Environment;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter.Native.Function
{
    public class LambdaInstance : RuntimeObject, ICallable
    {
        private Lazy<List<TypeMetadata>> _argumentsList;

        private PineTreeEnvironment _context;
        private LambdaExpression _lambda;

        public override TypeInfo TypeInfo => TypeInfo.Function;

        private List<TypeMetadata> _arguments => _argumentsList.Value;

        public LambdaInstance(PineTreeEngine engine, LambdaExpression method)
            : base(engine)
        {
            _lambda = method;
            _context = engine.ExecutionContext.Clone();
            _argumentsList = new Lazy<List<TypeMetadata>>(() =>
            {
                var list = new List<TypeMetadata>();

                foreach (var arg in method.Arguments)
                {
                    var type = arg.Type;
                    if (arg.Type == "var")
                    {
                        type = "Lambda_Argument";
                    }
                    list.Add(engine.FindType(type));
                }

                return list;
            });

            BindMethod("invoke", new Func<RuntimeValue>(() => Invoke(new RuntimeValue(this), new RuntimeValue[0])));
        }

        public bool ArgumentsMatch(TypeMetadata[] types)
        {
            return true;
        }

        public RuntimeValue Invoke(RuntimeValue thisBinding, RuntimeValue[] args)
        {
            Engine.SwitchContext(_context.Clone());

            foreach (var arg in _lambda.Arguments.Zip(args, (g, f) => new { Name = g.Name, Value = f }))
            {
                Engine.ExecutionContext.SetLocal(arg.Name, arg.Value);
            }

            var obj = Engine.Evaluate(_lambda.Body);

            Engine.PopExecutionContext();

            return obj.Value;
        }

        public override object ToClr()
        {
            return new PineTreeFunctionInvoke(Invoke);
        }

        public override string ToString()
        {
            return "lambda";
        }
    }
}