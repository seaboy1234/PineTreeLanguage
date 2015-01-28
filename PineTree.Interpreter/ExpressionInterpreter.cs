using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Extensions;
using PineTree.Interpreter.Native;
using PineTree.Interpreter.Native.Array;
using PineTree.Interpreter.Native.Boolean;
using PineTree.Interpreter.Native.Float;
using PineTree.Interpreter.Native.Function;
using PineTree.Interpreter.Native.Integer;
using PineTree.Interpreter.Native.String;
using PineTree.Interpreter.Runtime;
using PineTree.Interpreter.Runtime.Environment;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter
{
    public class ExpressionInterpreter
    {
        private PineTreeEngine _engine;

        public ExpressionInterpreter(PineTreeEngine interpreter)
        {
            _engine = interpreter;
        }

        public RuntimeValue EvaluateExpression(Expression expression)
        {
            switch (expression.SyntaxType)
            {
                case SyntaxTypes.ArithmeticExpression:
                    return EvaluateArithmetic(expression.As<ArithmeticExpression>());

                case SyntaxTypes.AssignmentExpression:
                    return EvaluateAssignment(expression.As<AssignmentExpression>());

                case SyntaxTypes.ConstantExpression:
                    return EvaluateConstant(expression.As<ConstantExpression>());

                case SyntaxTypes.LogicalExpression:
                    return EvaluateLogicalExpression(expression.As<LogicalExpression>());

                case SyntaxTypes.IdentifierExpression:
                    return EvaluateIdentifier(expression.As<IdentifierExpression>());

                case SyntaxTypes.NewExpression:
                    return EvaluateNew(expression.As<NewExpression>());

                case SyntaxTypes.MethodCallExpression:
                    return EvaluateMethodCall(expression.As<MethodCallExpression>());

                case SyntaxTypes.ObjectReferenceExpression:
                    return EvaluateObjectReference(expression.As<ObjectReferenceExpression>());

                case SyntaxTypes.LambdaExpression:
                    return EvaluateLambda(expression.As<LambdaExpression>());

                case SyntaxTypes.UnaryExpression:
                    return EvaluateUnaryExpression(expression.As<UnaryExpression>());

                case SyntaxTypes.ArrayAccessExpression:
                    return EvaluateArrayAccess(expression.As<ArrayAccessExpression>());

                case SyntaxTypes.ArrayDeclarationExpression:
                    return EvaluateArrayDeclaration(expression.As<ArrayDeclarationExpression>());

                case SyntaxTypes.CompoundExpression:
                    return EvaluateCompound(expression.As<CompoundExpression>());

                default:
                    throw new NotImplementedException();
            }
        }

        private RuntimeValue EvaluateArithmetic(ArithmeticExpression expression)
        {
            RuntimeValue left = EvaluateExpression(expression.Left);
            RuntimeValue right = EvaluateExpression(expression.Right);

            return left.EvaluateArithmetic(right, expression.Operation);
        }

        private RuntimeValue EvaluateArrayAccess(ArrayAccessExpression expression, RuntimeValue reference = default(RuntimeValue))
        {
            RuntimeValue value;
            if (reference.Value != null)
            {
                value = reference.GetValue(expression.Target);
            }
            else
            {
                value = _engine.GetValue(expression.Target);
            }

            return value.AccessArrayIndex(EvaluateExpression(expression.Index));
        }

        private RuntimeValue EvaluateArrayDeclaration(ArrayDeclarationExpression expression)
        {
            var size = ((IntegerInstance)EvaluateExpression(expression.Size).Value).Value;
            var type = _engine.ResolveType(expression.Type);
            ArrayInstance instance = new ArrayInstance(_engine, type, (int)size);

            return new RuntimeValue(instance);
        }

        private RuntimeValue EvaluateAssignment(AssignmentExpression expression, bool implicitDeclarationAllowed = false)
        {
            ObjectReference reference;
            RuntimeValue value = default(RuntimeValue);

            if (expression.Left.SyntaxType == SyntaxTypes.ObjectReferenceExpression)
            {
                ObjectReferenceExpression expr = expression.Left.As<ObjectReferenceExpression>();
                var root = EvaluateExpression(expr.Root);

                var referenceValue = root;

                foreach (var current in expr.References)
                {
                    if (current == expr.Final)
                    {
                        break;
                    }
                    if (current.SyntaxType == SyntaxTypes.IdentifierExpression)
                    {
                        referenceValue = referenceValue.GetValue(current.As<IdentifierExpression>().Identifier);
                    }
                    else if (current.SyntaxType == SyntaxTypes.MethodCallExpression)
                    {
                        referenceValue = EvaluateMethodCall(current.As<MethodCallExpression>(), referenceValue);
                    }
                    else if (current.SyntaxType == SyntaxTypes.ArrayAccessExpression)
                    {
                        referenceValue = EvaluateArrayAccess(current.As<ArrayAccessExpression>(), referenceValue);
                    }
                }
                value = EvaluateAssignmentRight(expression);
                if (expr.Final.SyntaxType == SyntaxTypes.ArrayAccessExpression)
                {
                    var array = expr.Final.As<ArrayAccessExpression>();
                    referenceValue.SetArrayIndex(EvaluateExpression(array.Index), value);
                }
                else
                {
                    referenceValue.SetValue(expr.Final.As<IdentifierExpression>().Identifier, value);
                }
                return value;
            }
            else if (expression.Left.SyntaxType == SyntaxTypes.ArrayAccessExpression)
            {
                ArrayAccessExpression expr = expression.Left.As<ArrayAccessExpression>();
                value = _engine.GetValue(expr.Target);
                value.SetArrayIndex(EvaluateExpression(expr.Index), EvaluateExpression(expression.Right));

                return value;
            }
            reference = _engine.GetReference(expression.Left.As<IdentifierExpression>().Identifier);

            if (reference == null)
            {
                if (!implicitDeclarationAllowed)
                {
                    throw new RuntimeException("No object with given path.");
                }

                string name = expression.Left.As<IdentifierExpression>().Identifier;

                _engine.SetValue(name, null);
                reference = _engine.GetReference(name);
            }

            value = EvaluateAssignmentRight(expression);

            reference.Value = value;

            return value;
        }

        private RuntimeValue EvaluateAssignmentRight(AssignmentExpression expression)
        {
            RuntimeValue value = default(RuntimeValue);
            switch (expression.Assignment)
            {
                case AssignmentType.AddTo:
                    value = EvaluateExpression(new ArithmeticExpression(expression.Left, expression.Right, ArithmeticOperator.Add));
                    break;

                case AssignmentType.SubractFrom:
                    value = EvaluateExpression(new ArithmeticExpression(expression.Left, expression.Right, ArithmeticOperator.Subtract));
                    break;

                case AssignmentType.DivideBy:
                    value = EvaluateExpression(new ArithmeticExpression(expression.Left, expression.Right, ArithmeticOperator.Divide));
                    break;

                case AssignmentType.MultiplyBy:
                    value = EvaluateExpression(new ArithmeticExpression(expression.Left, expression.Right, ArithmeticOperator.Multiply));
                    break;

                case AssignmentType.Regular:
                    value = EvaluateExpression(expression.Right);
                    break;
            }

            return value;
        }

        private RuntimeValue EvaluateCompound(CompoundExpression expression)
        {
            LexicalEnvironment env = _engine.CreateLexicalEnvironment();
            RuntimeValue last = RuntimeValue.Null;

            foreach (var expr in expression.Expressions)
            {
                if (expr.SyntaxType == SyntaxTypes.AssignmentExpression)
                {
                    last = EvaluateAssignment(expr.As<AssignmentExpression>(), true);
                }
                else
                {
                    last = EvaluateExpression(expr);
                }
            }

            _engine.PopLexicalEnvironment();

            return last;
        }

        private RuntimeValue EvaluateConstant(ConstantExpression constant)
        {
            switch (constant.Type)
            {
                case ConstantType.Int:
                    return new RuntimeValue(new IntegerInstance(_engine, int.Parse(constant.Value)));

                case ConstantType.Float:
                    return new RuntimeValue(new FloatInstance(_engine, float.Parse(constant.Value)));

                case ConstantType.Boolean:
                    return new RuntimeValue(new BooleanInstance(_engine, bool.Parse(constant.Value)));

                case ConstantType.String:
                    return new RuntimeValue(new StringInstance(_engine, constant.Value));

                case ConstantType.Null:
                default:
                    return RuntimeValue.Null;
            }
        }

        private RuntimeValue EvaluateIdentifier(IdentifierExpression identifierExpression)
        {
            return _engine.GetValue(identifierExpression.Identifier);
        }

        private RuntimeValue EvaluateLambda(LambdaExpression lambdaExpression)
        {
            var lambda = new LambdaInstance(_engine, lambdaExpression);

            return new RuntimeValue(lambda);
        }

        private RuntimeValue EvaluateLogicalExpression(LogicalExpression logicalExpression)
        {
            RuntimeValue left = EvaluateExpression(logicalExpression.Left);
            RuntimeValue right = EvaluateExpression(logicalExpression.Right);

            return left.EvaluateLogic(right, logicalExpression.Operation);
        }

        private RuntimeValue EvaluateMethodCall(MethodCallExpression call, RuntimeValue reference = default(RuntimeValue))
        {
            var arguments = call.Arguments.Select(g => EvaluateExpression(g));

            ICallable method = null;
            if (reference != RuntimeValue.Null)
            {
                method = reference.Value.ResolveMethod(call.Name, arguments.Select(g => _engine.FindType(g.TypeName)).ToArray());
            }
            else
            {
                method = _engine.FindLocalMethod(call.Name, arguments.Select(g => _engine.FindType(g.TypeName)).ToArray());
            }

            if (method == null)
            {
                throw new RuntimeException("Method not found.");
            }
            return method.Invoke(reference, arguments.ToArray());
        }

        private RuntimeValue EvaluateNew(NewExpression expression)
        {
            var arguments = expression.Arguments.Select(g => EvaluateExpression(g));

            TypeMetadata type = _engine.ResolveType(expression.Name);

            if (type == null)
            {
                throw new RuntimeException("Type not found.");
            }

            return type.CreateInstance(_engine, arguments.ToArray());
        }

        private RuntimeValue EvaluateObjectReference(ObjectReferenceExpression expression)
        {
            var root = EvaluateExpression(expression.Root);

            var reference = root;

            foreach (var current in expression.References)
            {
                if (current.SyntaxType == SyntaxTypes.IdentifierExpression)
                {
                    reference = reference.GetValue(current.As<IdentifierExpression>().Identifier);
                }
            }

            switch (expression.Final.SyntaxType)
            {
                case SyntaxTypes.MethodCallExpression:
                    return EvaluateMethodCall(expression.Final.As<MethodCallExpression>(), reference);

                case SyntaxTypes.IdentifierExpression:
                    return reference;
            }
            return RuntimeValue.Null;
        }

        private RuntimeValue EvaluateUnaryExpression(UnaryExpression unaryExpression)
        {
            var parameter = EvaluateExpression(unaryExpression.Parameter);

            return parameter.EvaluateUnaryExpression(unaryExpression.IsPrefix, unaryExpression.Operation);
        }
    }
}