using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language
{
    public enum TokenType
    {
        EOF,

        #region Trivia

        NewLine,
        Comment,
        WhiteSpace,
        Invalid,

        #endregion Trivia

        #region Literals

        Number,
        HexNumber,
        OctalNumber,
        FloatingPointNumber,

        StringLiteral,

        #endregion Literals

        #region Identifiers

        Identifier,
        Keyword,

        #endregion Identifiers

        #region Punctuation

        Semicolon,
        Dot,
        Comma,
        Colon,

        FatArrow,

        OpenPara,
        ClosePara,

        OpenBracket,
        CloseBracket,

        OpenArrayAccessor,
        CloseArrayAccessor,

        #endregion Punctuation

        #region Arithmetic Operators

        Multiply,
        Divide,
        Addition,
        Subtract,

        Modulo,

        PlusPlus,
        MinusMinus,

        #endregion Arithmetic Operators

        #region Assignment Operators

        AssignmentSubtract,
        AssignmentAdd,
        AssignmentDivide,
        AssignmentMultiply,
        Assignment,

        #endregion Assignment Operators

        #region Logical Operators

        LogicalLessOrEqual,
        LogicalLess,
        LogicalGreater,
        LogicalGreaterOrEqual,
        LogicalNotEquals,
        LogicalEquals,
        LogicalAnd,
        LogicalOr,
        LogicalNot,

        #endregion Logical Operators

        #region Bitwise Operators

        BitwiseXOR,
        BitwiseAnd,
        BitwiseOr,

        #endregion Bitwise Operators
    }
}