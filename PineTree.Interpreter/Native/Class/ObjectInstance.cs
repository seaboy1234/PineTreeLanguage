using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Interop;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Native.Class
{
    public class ObjectInstance : RuntimeObject
    {
        private TypeMetadata _type;

        public override TypeInfo TypeInfo => TypeInfo.Object;

        public override string TypeName => _type.Name;

        public ObjectInstance(PineTreeEngine engine, TypeMetadata type)
            : base(engine)
        {
            _type = type;
        }

        public override object ToClr()
        {
            return null;
        }

        public override string ToString()
        {
            return _type.Name;
        }
    }
}