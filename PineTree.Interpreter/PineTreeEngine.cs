using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Extensions;
using PineTree.Interpreter.Interop;
using PineTree.Interpreter.Native;
using PineTree.Interpreter.Native.Boolean;
using PineTree.Interpreter.Native.Class;
using PineTree.Interpreter.Native.Float;
using PineTree.Interpreter.Native.Function;
using PineTree.Interpreter.Native.Integer;
using PineTree.Interpreter.Native.String;
using PineTree.Interpreter.Runtime;
using PineTree.Interpreter.Runtime.Environment;
using PineTree.Language.Parser;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter
{
    public class PineTreeEngine
    {
        private static TypeRepository _TypeRepository;
        private Stack<PineTreeEnvironment> _callStack;
        private Module _currentModule;
        private ExpressionInterpreter _expressionInterpreter;
        private PineTreeParser _parser;
        private ExecutionContext _rootContext;
        private StatementInterpreter _statementInterpreter;

        public Module CurrentModule => _currentModule;

        public PineTreeEnvironment ExecutionContext => _callStack.Peek();

        public PineTreeEnvironment RootContext => _rootContext;

        public event EventHandler<ResolveModuleEventArgs> ResolveSpecialModule;

        static PineTreeEngine()
        {
            _TypeRepository = new TypeRepository();
        }

        public PineTreeEngine()
        {
            _parser = new PineTreeParser();
            _expressionInterpreter = new ExpressionInterpreter(this);
            _statementInterpreter = new StatementInterpreter(this);
            _callStack = new Stack<PineTreeEnvironment>();
            _rootContext = new ExecutionContext(CreateDynamicModule());
            _callStack.Push(_rootContext);
        }

        public Module CreateModule(string path)
        {
            var args = new ResolveModuleEventArgs(path);
            Module foundModule;

            ResolveSpecialModule?.Invoke(this, args);

            if (args.ResolvedModule != null)
            {
                foundModule = args.ResolvedModule;
            }
            else
            {
                path = path.Replace('.', Path.DirectorySeparatorChar) + ".pt";

                var module = _currentModule;
                foundModule = CreateDynamicModule();

                Execute(File.ReadAllText(path));

                _currentModule = module;
            }

            return foundModule;
        }

        public Completion Evaluate(SyntaxNode syntaxNode)
        {
            if (syntaxNode == null)
            {
                return null;
            }
            if (syntaxNode is Statement)
            {
                return _statementInterpreter.Evaluate(syntaxNode.As<Statement>());
            }
            else if (syntaxNode is Expression)
            {
                return new Completion(false, _expressionInterpreter.EvaluateExpression(syntaxNode.As<Expression>()));
            }
            else if (syntaxNode is ClassDeclaration)
            {
                _currentModule.DefineType(new ClassMetadata(this, syntaxNode.As<ClassDeclaration>()));

                return new Completion(true, new RuntimeValue(new StringInstance(this, "class")));
            }
            else if (syntaxNode is MethodDeclaration)
            {
                FunctionInstance func = new FunctionInstance(this, syntaxNode.As<MethodDeclaration>());
                _currentModule.BindMethod(func.Name, func);

                return new Completion(true, new RuntimeValue(func));
            }
            else if (syntaxNode is LexicalScope)
            {
                Completion last = null;
                CreateLexicalEnvironment();

                try
                {
                    foreach (var node in syntaxNode.As<LexicalScope>().Statements)
                    {
                        last = Evaluate(node);
                        if (last.ShouldReturn)
                        {
                            break;
                        }
                    }
                }
                finally
                {
                    PopLexicalEnvironment();
                }

                return last;
            }
            else if (syntaxNode is SourceDocument)
            {
                Completion last = null;

                foreach (var node in syntaxNode.As<SourceDocument>().Contents)
                {
                    last = Evaluate(node);
                }

                return last;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public RuntimeValue Execute(string sourceCode)
        {
            return Evaluate(_parser.ParseScript(sourceCode)).Value;
        }

        public ObjectReference GetReference(string name)
        {
            return ExecutionContext.GetReference(name);
        }

        public RuntimeValue GetValue(string name)
        {
            return ExecutionContext.GetLocal(name);
        }

        public void ImportModule(Module module)
        {
            _rootContext.AddModule(module);
        }

        public bool IsDefined(string name)
        {
            return ExecutionContext.IsDefined(name);
        }

        public void SetValue(string name, object value)
        {
            RuntimeValue runtimeValue;
            if (value == null)
            {
                runtimeValue = RuntimeValue.Null;
            }
            else if (value is Delegate)
            {
                runtimeValue = new RuntimeValue(new ExternalMethod(this, (Delegate)value));
            }
            else
            {
                runtimeValue = CastObject(value);
            }

            ExecutionContext.SetLocal(name, runtimeValue);
        }

        public void SetValue(string name, RuntimeValue value)
        {
            ExecutionContext.SetLocal(name, value);
        }

        internal RuntimeValue CastObject(object obj)
        {
            RuntimeObject value = null;
            if (obj is RuntimeValue)
            {
                return (RuntimeValue)obj;
            }
            else if (obj is RuntimeObject)
            {
                return new RuntimeValue(obj as RuntimeObject);
            }
            else if (obj is string)
            {
                value = new StringInstance(this, (string)obj);
            }
            else if (obj is int || obj is short || obj is sbyte || obj is long)
            {
                value = new IntegerInstance(this, (long)obj);
            }
            else if (obj is float || obj is double)
            {
                value = new FloatInstance(this, (double)obj);
            }
            else if (obj is bool)
            {
                value = new BooleanInstance(this, (bool)obj);
            }
            else
            {
                value = new ClrObjectInstance(this, obj);
            }

            return new RuntimeValue(value);
        }

        internal void CreateExecutionContext(RuntimeValue thisBinding)
        {
            _callStack.Push(new FunctionCallContext(this, thisBinding.Value));
        }

        internal LexicalEnvironment CreateLexicalEnvironment()
        {
            LexicalEnvironment environment;
            if (ExecutionContext is FunctionCallContext)
            {
                environment = ((FunctionCallContext)ExecutionContext).PushScope();
            }
            else
            {
                environment = new LexicalEnvironment(ExecutionContext);
                _callStack.Push(environment);
            }

            return environment;
        }

        internal ICallable FindLocalMethod(string name, TypeMetadata[] types)
        {
            var local = ExecutionContext.GetLocal(name).Value;
            if (local != null && local is ICallable && (((ICallable)local)?.ArgumentsMatch(types) ?? false))
            {
                return (ICallable)local;
            }
            return _currentModule.FindMethod(name, types);
        }

        internal TypeMetadata FindType(string name)
        {
            return _TypeRepository.FindType(name);
        }

        internal TypeMetadata FindType(RuntimeValue value)
        {
            return FindType(value.TypeName);
        }

        internal void PopExecutionContext()
        {
            _callStack.Pop();
        }

        internal void PopLexicalEnvironment()
        {
            if (ExecutionContext is FunctionCallContext)
            {
                ((FunctionCallContext)ExecutionContext).PopScope();
            }
            else
            {
                _callStack.Pop();
            }
        }

        internal TypeMetadata ResolveType(string name)
        {
            return _rootContext.FindType(name) ?? _TypeRepository.FindType(name);
        }

        internal void SwitchContext(PineTreeEnvironment context)
        {
            _callStack.Push(context);
        }

        private Module CreateDynamicModule()
        {
            return CreateNamedModule("Script");
        }

        private Module CreateNamedModule(string name)
        {
            _currentModule = new Module(this, name);
            return _currentModule;
        }
    }
}
