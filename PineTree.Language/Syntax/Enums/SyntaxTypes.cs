using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Syntax
{
    public enum SyntaxTypes
    {
        #region Expressions

        LambdaExpression,
        CompoundExpression,

        // Binary Expressions
        ArithmeticExpression,

        AssignmentExpression,
        BitwiseExpression,
        LogicalExpression,

        // Unary Expressions
        ConstantExpression,

        UnaryExpression,
        IdentifierExpression,

        MethodCallExpression,
        ArrayAccessExpression,
        ArrayDeclarationExpression,
        NewExpression,

        ObjectReferenceExpression,

        #endregion Expressions

        #region Statements

        EmptyStatement,
        ElseStatement,
        ForStatement,
        IfStatement,
        LexicalScope,
        WhileStatement,
        ReturnStatement,
        ImportStatement,

        PreconditionStatement,

        #endregion Statements

        #region Declarations

        ClassDeclaration,
        ConstructorDeclaration,
        FieldDeclaration,
        MethodDeclaration,
        PropertyDeclaration,
        VariableDeclaration,

        #endregion Declarations

        SourceDocument,
    }
}