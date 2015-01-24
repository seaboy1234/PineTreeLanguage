using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Extensions;
using PineTree.Interpreter.Runtime;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter
{
    public class StatementInterpreter
    {
        private PineTreeEngine _engine;

        public StatementInterpreter(PineTreeEngine engine)
        {
            _engine = engine;
        }

        public Completion Evaluate(Statement statement)
        {
            switch (statement.SyntaxType)
            {
                case SyntaxTypes.VariableDeclaration:
                    return DeclareVariable(statement.As<VariableDeclaration>());

                case SyntaxTypes.ReturnStatement:
                    return EvaluateReturn(statement.As<ReturnStatement>());

                default:
                    return new Completion(false, RuntimeValue.Null);
            }
        }

        private Completion DeclareVariable(VariableDeclaration variableDeclaration)
        {
            if (_engine.IsDefined(variableDeclaration.Name))
            {
                throw new RuntimeException("\{variableDeclaration.Name} is already defined.");
            }

            RuntimeValue value = RuntimeValue.Null;

            if (variableDeclaration.Value != null)
            {
                value = _engine.Evaluate(variableDeclaration.Value).Value;
            }

            _engine.SetValue(variableDeclaration.Name, value);
            return new Completion(false, value);
        }

        private Completion EvaluateReturn(ReturnStatement statement)
        {
            RuntimeValue value = _engine.Evaluate(statement.Expression).Value;

            return new Completion(true, value);
        }
    }
}