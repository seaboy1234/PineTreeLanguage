using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Interop;
using PineTree.Interpreter.Native;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter
{
    public class Module : RuntimeObject
    {
        private TypeRepository _types;

        public string Name { get; }

        public override TypeInfo TypeInfo => TypeInfo.Object;

        public Module(PineTreeEngine engine, string name)
            : base(engine)
        {
            _types = new TypeRepository();
            Name = name;
        }

        public static Module CreateSpecial(PineTreeEngine engine, string name, IDictionary<string, object> contents)
        {
            var module = new Module(engine, name);

            foreach (var item in contents)
            {
                module.BindObject(item.Key, item.Value);
            }

            return module;
        }

        public void BindObject<T>(string name, T value)
        {
            BindProperty(name, new ExternalMethod(Engine, new Func<T>(() => value)));
        }

        public void DefineType(TypeMetadata type)
        {
            _types.DefineType(type);
        }

        public TypeMetadata FindType(string name)
        {
            return _types.FindType(name);
        }

        public override object ToClr()
        {
            return this;
        }
    }
}
