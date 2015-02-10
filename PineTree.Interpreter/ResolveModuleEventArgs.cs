using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Interpreter
{
	public class ResolveModuleEventArgs : EventArgs
	{
		public string Name { get; }

		public Module ResolvedModule { get; set; }

		public ResolveModuleEventArgs(string name)
		{
			Name = name;
		}

		public ResolveModuleEventArgs(string name, Module module)
			: this(name)
		{
			ResolvedModule = module;
		}
	}
}
