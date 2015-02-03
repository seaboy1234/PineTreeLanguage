using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Interop;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Native.Error
{
    public class ErrorInstance : RuntimeObject
    {
        private Lazy<RuntimeException> _exception;

        public RuntimeException Exception => _exception.Value;

        public override TypeInfo TypeInfo => TypeInfo.Error;

        public ErrorInstance(PineTreeEngine engine)
            : base(engine)
        {
            _exception = new Lazy<RuntimeException>();
        }

        public ErrorInstance(PineTreeEngine engine, string message)
            : base(engine)
        {
            _exception = new Lazy<RuntimeException>(() => new RuntimeException(message, this));
            BindMethods();
        }

        public ErrorInstance(PineTreeEngine engine, RuntimeException exception)
            : base(engine)
        {
            _exception = new Lazy<RuntimeException>(() => exception);
            BindMethods();
        }

        public override object ToClr()
        {
            return Exception;
        }

        private void BindMethods()
        {
            BindProperty("message", Exception.Message);
        }

        private void BindProperty<T>(string name, T value)
        {
            base.BindProperty(name, new ExternalMethod(Engine, new Func<T>(() => value)));
        }
    }
}