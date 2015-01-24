using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Runtime;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter.Native.Function
{
    public class ConstructorInstance : ICallable
    {
        private readonly ConstructorDeclaration _methodInfo;
        private Lazy<List<TypeMetadata>> _argumentsList;
        private PineTreeEngine _engine;

        public ConstructorDeclaration Declaration => _methodInfo;

        public string Name => _methodInfo.Name;

        private List<TypeMetadata> _arguments => _argumentsList.Value;

        public ConstructorInstance(PineTreeEngine engine, ConstructorDeclaration method)
        {
            _engine = engine;
            _methodInfo = method;

            _argumentsList = new Lazy<List<TypeMetadata>>(() =>
            {
                var list = new List<TypeMetadata>();

                foreach (var arg in method.Arguments)
                {
                    list.Add(engine.FindType(arg.Type));
                }

                return list;
            });
        }

        public bool ArgumentsMatch(TypeMetadata[] types)
        {
            if (_arguments.Any(g => g == null))
            {
                throw new RuntimeException("Missing type");
            }
            return types.SequenceEqual(_arguments);
        }

        public RuntimeValue Invoke(RuntimeValue thisBinding, RuntimeValue[] args)
        {
            _engine.CreateExecutionContext(thisBinding);

            foreach (var arg in _methodInfo.Arguments.Zip(args, (g, f) => new { Name = g.Name, Value = f }))
            {
                _engine.ExecutionContext.SetLocal(arg.Name, arg.Value);
            }

            var obj = _engine.Evaluate(_methodInfo.Body);

            _engine.PopExecutionContext();

            return obj.Value;
        }
    }
}