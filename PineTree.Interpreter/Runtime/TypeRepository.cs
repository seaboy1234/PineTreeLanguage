using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native.Array;
using PineTree.Interpreter.Native.Boolean;
using PineTree.Interpreter.Native.Error;
using PineTree.Interpreter.Native.Float;
using PineTree.Interpreter.Native.Function;
using PineTree.Interpreter.Native.Integer;
using PineTree.Interpreter.Native.String;

namespace PineTree.Interpreter.Runtime
{
    public class TypeRepository
    {
        private static List<TypeMetadata> _Types;
        private List<TypeMetadata> _types;

        static TypeRepository()
        {
            _Types = new List<TypeMetadata>()
            {
                new BooleanType(),
                new FloatType(),
                new FunctionType(),
                new IntegerType(),
                new StringType(),
                new LamdaArgumentType(),
                new ArrayType(),
                new ErrorType(),
            };
        }

        public TypeRepository()
        {
            _types = new List<TypeMetadata>();
        }

        public void DefineType(TypeMetadata metadata)
        {
            _types.Add(metadata);
        }

        public TypeMetadata FindType(string name)
        {
            return _types.Find(g => g.Name == name) ?? _Types.Find(g => g.Name == name);
        }
    }
}