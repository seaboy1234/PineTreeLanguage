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

                case SyntaxTypes.IfStatement:
                    return EvaluateIf(statement.As<IfStatement>());

                case SyntaxTypes.WhileStatement:
                    return EvaluateWhile(statement.As<WhileStatement>());

                case SyntaxTypes.ForStatement:
                    return EvaluateFor(statement.As<ForStatement>());

                default:
                    throw new NotImplementedException();
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

        private Completion EvaluateFor(ForStatement statement)
        {
            _engine.CreateLexicalEnvironment();
            _engine.Evaluate(statement.Variable);
            Completion completion = null;
            while ((bool)_engine.Evaluate(statement.Predicate).Value.ToClr())
            {
                completion = _engine.Evaluate(statement.Body);

                if (completion.ShouldReturn)
                {
                    break;
                }

                _engine.Evaluate(statement.Increment);
            }
            _engine.PopLexicalEnvironment();
            return completion ?? new Completion(false, RuntimeValue.Null);
        }

        private Completion EvaluateIf(IfStatement ifStatement)
        {
            var predicateValue = _engine.Evaluate(ifStatement.Predicate);

            if (predicateValue.Value.TypeInfo != TypeInfo.Boolean)
            {
                throw new RuntimeException("Predicate must return a boolean!");
            }

            if ((bool)predicateValue.Value.ToClr())
            {
                Completion completion = _engine.Evaluate(ifStatement.Body);

                return completion;
            }
            else if (ifStatement.ElseStatement != null)
            {
                return _engine.Evaluate(ifStatement.ElseStatement);
            }
            return new Completion(false, RuntimeValue.Null);
        }

        private Completion EvaluateReturn(ReturnStatement statement)
        {
            RuntimeValue value = _engine.Evaluate(statement.Expression).Value;

            return new Completion(true, value);
        }

        private Completion EvaluateWhile(WhileStatement whileStatement)
        {
            Completion completion = null;
            while ((bool)_engine.Evaluate(whileStatement.Predicate).Value.ToClr())
            {
                completion = _engine.Evaluate(whileStatement.Body);
                if (completion.ShouldReturn)
                {
                    return completion;
                }
            }
            return completion ?? new Completion(false, RuntimeValue.Null);
        }
    }
}