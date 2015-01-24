using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public class ClassDeclaration : SyntaxNode
    {
        public IEnumerable<ConstructorDeclaration> Constructors { get; }

        public IEnumerable<FieldDeclaration> Fields { get; }

        public IEnumerable<MethodDeclaration> Methods { get; }

        public string Name { get; }

        public IEnumerable<PropertyDeclaration> Properties { get; }

        public string Supertype { get; }

        public override SyntaxTypes SyntaxType => SyntaxTypes.ClassDeclaration;

        public ClassDeclaration(string name, string supertype, IEnumerable<FieldDeclaration> fields, IEnumerable<PropertyDeclaration> properties, IEnumerable<MethodDeclaration> methods, IEnumerable<ConstructorDeclaration> constructors)
        {
            Name = name;
            Supertype = supertype;
            Properties = properties;
            Fields = fields;
            Constructors = constructors;
            Methods = methods;
        }
    }
}