using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native.Integer;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter.Native.Array
{
    public class ArrayInstance : RuntimeObject
    {
        private RuntimeObject[] _array;
        private TypeMetadata _type;

        public override TypeInfo TypeInfo => TypeInfo.Array;

        public ArrayInstance(PineTreeEngine engine, TypeMetadata type, int size)
                    : base(engine)
        {
            _array = new RuntimeObject[size];
            _type = type;
        }

        public override RuntimeValue AccessArray(RuntimeValue index)
        {
            IntegerInstance intdex;
            if (!(index.Value is IntegerInstance))
            {
                return RuntimeValue.Null;
            }
            intdex = (IntegerInstance)index.Value;

            return new RuntimeValue(_array[intdex.Value]);
        }

        public override void SetArrayValue(RuntimeValue index, RuntimeValue value)
        {
            IntegerInstance intdex;
            if (!(index.Value is IntegerInstance))
            {
                return;
            }
            intdex = (IntegerInstance)index.Value;

            if (value.TypeName != _type.Name)
            {
                return;
            }

            _array[intdex.Value] = value.Value;
        }

        public override object ToClr()
        {
            return _array.Select(g => g.ToClr()).ToArray();
        }

        public override string ToString()
        {
            return "\{_type.Name}[\{_array.Length}]";
        }
    }
}