using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Interpreter;

namespace PineTree
{
    public class MathModule : Module
    {
        private class MathObject
        {
            public double Sqrt(double value)
            {
                return Math.Sqrt(value);
            }
        }

        public MathModule(PineTreeEngine engine) : base(engine)
        {
            BindObject("Math", new MathObject());
        }
    }
}
