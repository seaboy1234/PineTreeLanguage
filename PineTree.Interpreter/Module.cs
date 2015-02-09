using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter.Native;
using PineTree.Interpreter.Runtime;

namespace PineTree.Interpreter
{
	public class Module : RuntimeObject
	{
		private TypeRepository _types;

		public override TypeInfo TypeInfo => TypeInfo.Object;

		public Module(PineTreeEngine engine)
			: base(engine)
		{
			_types = new TypeRepository();
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
