using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using PineTree.DomGenerator.Extensions;
using PineTree.Language.Syntax;

namespace PineTree.DomGenerator
{
    public class PineTreeDocumentGenerator
    {
        public XDocument CreateDocument(SourceDocument file)
        {
            return new XDocument(Generate(file));
        }

        private XElement Generate(SyntaxNode node)
        {
            if (node == null)
            {
                return null;
            }
            switch (node.SyntaxType)
            {
                case SyntaxTypes.ArithmeticExpression:
                    return GenerateArithmetic(node.As<ArithmeticExpression>());

                case SyntaxTypes.AssignmentExpression:
                    return GenerateAssignment(node.As<AssignmentExpression>());

                case SyntaxTypes.BitwiseExpression:
                    return GenerateBitwise(node.As<BitwiseExpression>());

                case SyntaxTypes.ClassDeclaration:
                    return GenerateClass(node.As<ClassDeclaration>());

                case SyntaxTypes.ConstantExpression:
                    return GenerateConstant(node.As<ConstantExpression>());

                case SyntaxTypes.ConstructorDeclaration:
                    return GenerateConstructor(node.As<ConstructorDeclaration>());

                case SyntaxTypes.ElseStatement:
                    return GenerateElseStatement(node.As<ElseStatement>());

                case SyntaxTypes.EmptyStatement:
                    return GenerateEmptyStatement(node.As<EmptyStatement>());

                case SyntaxTypes.FieldDeclaration:
                    return GenerateFieldDeclaration(node.As<FieldDeclaration>());

                case SyntaxTypes.ForStatement:
                    break;

                case SyntaxTypes.IdentifierExpression:
                    return GenerateIdentifier(node.As<IdentifierExpression>());

                case SyntaxTypes.IfStatement:
                    return GenerateIfStatement(node.As<IfStatement>());

                case SyntaxTypes.ImportStatement:
                    return GenerateImport(node.As<ImportStatement>());

                case SyntaxTypes.LexicalScope:
                    return GenerateLexicalScope(node.As<LexicalScope>());

                case SyntaxTypes.LogicalExpression:
                    return GenerateLogic(node.As<LogicalExpression>());

                case SyntaxTypes.MethodCallExpression:
                    return GenerateMethodCall(node.As<MethodCallExpression>());

                case SyntaxTypes.MethodDeclaration:
                    return GenerateMethodDeclaration(node.As<MethodDeclaration>());

                case SyntaxTypes.NewExpression:
                    return GenerateNew(node.As<NewExpression>());

                case SyntaxTypes.ObjectReferenceExpression:
                    return GenerateObjectReference(node.As<ObjectReferenceExpression>());

                case SyntaxTypes.PropertyDeclaration:
                    return GeneratePropertyDeclaration(node.As<PropertyDeclaration>());

                case SyntaxTypes.ReturnStatement:
                    return GenerateReturnStatement(node.As<ReturnStatement>());

                case SyntaxTypes.SourceDocument:
                    return GenerateSourceDocument(node.As<SourceDocument>());

                case SyntaxTypes.UnaryExpression:
                    return GenerateUnaryExpression(node.As<UnaryExpression>());

                case SyntaxTypes.VariableDeclaration:
                    return GenerateVariable(node.As<VariableDeclaration>());

                case SyntaxTypes.LambdaExpression:
                    return GenerateLambda(node.As<LambdaExpression>());

                case SyntaxTypes.WhileStatement:
                    break;
            }
            return null;
        }

        private IEnumerable<XElement> Generate<T>(IEnumerable<T> nodes)
            where T : SyntaxNode
        {
            foreach (var node in nodes)
            {
                yield return Generate(node);
            }
        }

        private XElement GenerateArithmetic(ArithmeticExpression arithmeticExpression)
        {
            return new XElement("ArithmeticExpression",
                Generate(arithmeticExpression.Left),
                Generate(arithmeticExpression.Right),
                new XAttribute("Operator", arithmeticExpression.Operation));
        }

        private XElement GenerateAssignment(AssignmentExpression assignmentExpression)
        {
            return new XElement("AssignmentExpression",
                new XElement("Left", Generate(assignmentExpression.Left)),
                new XElement("Right", Generate(assignmentExpression.Right)),
                new XAttribute("Operator", assignmentExpression.Assignment));
        }

        private XElement GenerateBitwise(BitwiseExpression bitwiseExpression)
        {
            return new XElement("BitwiseExpression",
                Generate(bitwiseExpression.Left),
                Generate(bitwiseExpression.Right),
                new XAttribute("Operator", bitwiseExpression.Operation));
        }

        private XElement GenerateClass(ClassDeclaration classDeclaration)
        {
            return new XElement("ClassDeclaration",
                Generate(classDeclaration.Fields),
                Generate(classDeclaration.Properties),
                Generate(classDeclaration.Constructors),
                Generate(classDeclaration.Methods),
                new XAttribute("Name", classDeclaration.Name),
                new XAttribute("Extends", classDeclaration.Supertype));
        }

        private XElement GenerateConstant(ConstantExpression constantExpression)
        {
            return new XElement("ConstantExpression",
                constantExpression.Value,
                new XAttribute("Type", constantExpression.Type));
        }

        private XElement GenerateConstructor(ConstructorDeclaration constructorDeclaration)
        {
            return new XElement("ConstructorDeclaration",
                new XElement("Arguments", Generate(constructorDeclaration.Arguments)),
                new XElement("Body", Generate(constructorDeclaration.Body)),
                new XAttribute("Visibility", constructorDeclaration.Visibility));
        }

        private XElement GenerateElseStatement(ElseStatement elseStatement)
        {
            return new XElement("ElseStatement",
                Generate(elseStatement.Body));
        }

        private XElement GenerateEmptyStatement(EmptyStatement emptyStatement)
        {
            return new XElement("EmptyStatement");
        }

        private XElement GenerateFieldDeclaration(FieldDeclaration fieldDeclaration)
        {
            return new XElement("FieldDeclaration",
                new XAttribute("Name", fieldDeclaration.Name),
                new XAttribute("Visibility", "private"),
                new XElement("Value", Generate(fieldDeclaration.Value)));
        }

        private XElement GenerateIdentifier(IdentifierExpression identifierExpression)
        {
            return new XElement("IdentifierExpression", identifierExpression.Identifier);
        }

        private XElement GenerateIfStatement(IfStatement ifStatement)
        {
            return new XElement("IfStatement",
                new XElement("Condition", Generate(ifStatement.Predicate)),
                new XElement("Body", Generate(ifStatement.Body)),
                Generate(ifStatement.ElseStatement));
        }

        private XElement GenerateImport(ImportStatement importStatement)
        {
            return new XElement("ImportStatement", importStatement.API);
        }

        private XElement GenerateLambda(LambdaExpression lambdaDeclarationExpression)
        {
            return new XElement("LambdaDeclaration",
                new XElement("Arguments", Generate(lambdaDeclarationExpression.Arguments)),
                new XElement("Body", Generate(lambdaDeclarationExpression.Body)));
        }

        private XElement GenerateLexicalScope(LexicalScope lexicalScope)
        {
            return new XElement("LexicalScope", Generate(lexicalScope.Statements));
        }

        private XElement GenerateLogic(LogicalExpression logicalExpression)
        {
            return new XElement("LogicalExpression",
                Generate(logicalExpression.Left),
                Generate(logicalExpression.Right),
                new XAttribute("Operator", logicalExpression.Operation));
        }

        private XElement GenerateMethodCall(MethodCallExpression methodCallExpression)
        {
            return new XElement("MethodCallExpression",
                new XElement("Arguments", Generate(methodCallExpression.Arguments)),
                new XAttribute("Name", methodCallExpression.Name));
        }

        private XElement GenerateMethodDeclaration(MethodDeclaration methodDeclaration)
        {
            return new XElement("MethodDeclaration",
                new XElement("Arguments", Generate(methodDeclaration.Arguments)),
                new XElement("Body", Generate(methodDeclaration.Body)),
                new XAttribute("Name", methodDeclaration.Name),
                new XAttribute("ReturnType", methodDeclaration.ReturnType),
                new XAttribute("Visibility", methodDeclaration.Visibility));
        }

        private XElement GenerateNew(NewExpression newExpression)
        {
            return new XElement("NewExpression",
                new XElement("Arguments", Generate(newExpression.Arguments)),
                new XAttribute("Type", newExpression.Name));
        }

        private XElement GenerateObjectReference(ObjectReferenceExpression objectReferenceExpression)
        {
            return new XElement("ObjectReferenceExpression",
                new XElement("Root", Generate(objectReferenceExpression.Root)),
                new XElement("References", Generate(objectReferenceExpression.References)));
        }

        private XElement GeneratePropertyDeclaration(PropertyDeclaration propertyDeclaration)
        {
            return new XElement("PropertyDeclaration",
                new XElement("Get", Generate(propertyDeclaration.GetMethod)),
                new XElement("Set", Generate(propertyDeclaration.SetMethod)),
                new XAttribute("Name", propertyDeclaration.Name),
                new XAttribute("Type", propertyDeclaration.Type),
                new XAttribute("Visibility", propertyDeclaration.Visibility));
        }

        private XElement GenerateReturnStatement(ReturnStatement returnStatement)
        {
            return new XElement("ReturnStatement",
                Generate(returnStatement.Expression));
        }

        private XElement GenerateSourceDocument(SourceDocument sourceDocument)
        {
            return new XElement("Document",
                Generate(sourceDocument.Contents));
        }

        private XElement GenerateUnaryExpression(UnaryExpression unaryExpression)
        {
            return new XElement("UnaryExpression",
                Generate(unaryExpression.Parameter),
                new XAttribute("Prefix", unaryExpression.IsPrefix));
        }

        private XElement GenerateVariable(VariableDeclaration variableDeclaration)
        {
            return new XElement("VariableDeclaration",
                Generate(variableDeclaration.Value),
                new XAttribute("Name", variableDeclaration.Name),
                new XAttribute("Type", variableDeclaration.Type));
        }
    }
}