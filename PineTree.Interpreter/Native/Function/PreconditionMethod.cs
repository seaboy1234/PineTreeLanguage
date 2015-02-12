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
    public class PreconditionMethod : RuntimeObject, ICallable
    {
        private ICallable _parent;
        private IEnumerable<PreconditionStatement> _preconditions;

        public override TypeInfo TypeInfo => TypeInfo.Function;

        public PreconditionMethod(PineTreeEngine engine, ICallable parent, IEnumerable<PreconditionStatement> preconditions)
            : base(engine)
        {
            _preconditions = preconditions;
        }

        public bool ArgumentsMatch(TypeMetadata[] types)
        {
            return _parent.ArgumentsMatch(types);
        }

        public RuntimeValue Invoke(RuntimeValue thisBinding, RuntimeValue[] args)
        {
            foreach (var precondition in _preconditions)
            {
                try
                {
                    Engine.CreateLexicalEnvironment();
                    var output = Engine.Evaluate(precondition.Condition);
                    if (!(output.Value.Value is BooleanInstance))
                    {
                        throw new RuntimeException("Preconditions must return a boolean.");
                    }
                    if (!((bool)output.Value.ToClr()))
                    {
                        throw new RuntimeException(precondition.ErrorMessage);
                    }
                }
                finally
                {
                    Engine.PopLexicalEnvironment();
                }
            }
            return RuntimeValue.Null;
        }

        public override object ToClr()
        {
            return this;
        }
    }
}
