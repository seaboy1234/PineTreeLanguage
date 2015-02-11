using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native.Error;
using PineTree.Interpreter.Native.Function;
using PineTree.Interpreter.Runtime;
using PineTree.Language.Syntax;

namespace PineTree.Interpreter.Native.Class
{
    public class ClassMetadata : TypeMetadata
    {
        private List<ICallable> _constructors;
        private PineTreeEngine _engine;
        private List<FieldDeclaration> _fields;
        private List<FunctionInstance> _methods;
        private List<PropertyReference> _properties;
        private List<FunctionInstance> _staticMethods;

        public override TypeInfo Info => TypeInfo.Object;

        public override string Name { get; }

        public string ParentType { get; }

        public ClassMetadata(PineTreeEngine engine, ClassDeclaration declaration)
        {
            _constructors = new List<ICallable>();
            _fields = new List<FieldDeclaration>();
            _methods = new List<FunctionInstance>();
            _properties = new List<PropertyReference>();

            _staticMethods = new List<FunctionInstance>();

            _engine = engine;

            ImportClass(declaration);
            Name = declaration.Name;
            ParentType = declaration.Supertype;

            engine.CurrentModule.BindObject(Name, CreateStaticObject(engine, null));
        }

        public override bool CanCastTo(TypeMetadata typeMetadata)
        {
            return typeMetadata.Name == Name;
        }

        public override RuntimeValue CreateInstance(PineTreeEngine engine, RuntimeValue[] args)
        {
            RuntimeObject instance;
            if (ParentType == "Error")
            {
                instance = new ErrorInstance(engine);
            }
            else
            {
                instance = new ObjectInstance(engine, this);
            }

            if (!string.IsNullOrEmpty(ParentType))
            {
                (engine.ResolveType(ParentType) as ClassMetadata)?.ImportMembers(engine, instance);
            }
            ImportMembers(engine, instance);

            var thisBinding = new RuntimeValue(instance);

            if (_constructors.Count > 0)
            {
                var cotr = _constructors.Find(g => g.ArgumentsMatch(args.Select(f => engine.FindType(f)).ToArray()));

                if (cotr != null)
                {
                    cotr.Invoke(thisBinding, args);
                }
                else
                {
                    throw new RuntimeException("Constructor not found.");
                }
            }

            return thisBinding;
        }

        public RuntimeObject ImportMembers(PineTreeEngine engine, RuntimeObject instance)
        {
            if (instance == null)
            {
                instance = new ObjectInstance(engine, this);
            }
            foreach (var field in _fields)
            {
                instance.BindField(field.Name, engine.Evaluate(field.Value)?.Value ?? RuntimeValue.Null);
            }

            foreach (var property in _properties)
            {
                instance.BindProperty(property);
            }

            foreach (var method in _methods)
            {
                instance.BindMethod(method.Name, method);
            }
            return instance;
        }

        private RuntimeObject CreateStaticObject(PineTreeEngine engine, RuntimeObject instance)
        {
            if (instance == null)
            {
                instance = new ObjectInstance(engine, this);
            }

            if (ParentType != "object")
            {
                var type = engine.ResolveType(ParentType);
                if (type is ClassMetadata)
                {
                    ((ClassMetadata)type).CreateStaticObject(engine, instance);
                }
            }

            foreach (var method in _staticMethods)
            {
                instance.BindMethod(method.Name, method);
            }

            return instance;
        }

        private void ImportClass(ClassDeclaration declaration)
        {
            foreach (var field in declaration.Fields)
            {
                _fields.Add(field);
            }
            foreach (var constructor in declaration.Constructors)
            {
                ConstructorInstance func = new ConstructorInstance(_engine, constructor);
                _constructors.Add(func);
            }
            foreach (var property in declaration.Properties)
            {
                FunctionInstance setMethod = property.SetMethod == null ? null : new FunctionInstance(_engine, property.SetMethod);
                PropertyReference propertyReference = new PropertyReference(property.Name, new FunctionInstance(_engine, property.GetMethod), setMethod);

                _properties.Add(propertyReference);
            }
            foreach (var method in declaration.Methods)
            {
                var function = new FunctionInstance(_engine, method);
                if (method.IsStatic)
                {
                    _staticMethods.Add(function);
                }
                else
                {
                    _methods.Add(function);
                }
            }
        }
    }
}