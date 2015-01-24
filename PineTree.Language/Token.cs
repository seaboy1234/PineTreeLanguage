using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language
{
    public class Token : IEquatable<TokenType>, IEquatable<string>
    {
        private Lazy<TokenCatagory> _catagory;
        private TextSpan _span;

        public TokenCatagory Catagory => _catagory.Value;

        public Position End { get; }

        public TextSpan Span => _span;

        public Position Start { get; }

        public TokenType Type { get; }

        public string Value { get; }

        public Token(string value, TokenType type)
        {
            Value = value;
            Type = type;
        }

        public Token(string value, TokenType type, Position start, Position end)
            : this(value, type)
        {
            Start = start;
            End = end;
            _catagory = new Lazy<TokenCatagory>(() => GetCatagory(type));
            _span = new TextSpan(start, end);
        }

        public static TokenCatagory GetCatagory(TokenType type)
        {
            switch (type)
            {
                case TokenType.WhiteSpace:
                case TokenType.Comment:
                case TokenType.NewLine:
                    return TokenCatagory.Trivia;

                case TokenType.Number:
                case TokenType.HexNumber:
                case TokenType.OctalNumber:
                case TokenType.FloatingPointNumber:
                case TokenType.StringLiteral:
                    return TokenCatagory.Literal;

                case TokenType.Identifier:
                case TokenType.Keyword:
                    return TokenCatagory.Identifier;

                case TokenType.Semicolon:
                case TokenType.Dot:
                case TokenType.Comma:
                case TokenType.Colon:
                case TokenType.OpenPara:
                case TokenType.ClosePara:
                case TokenType.OpenBracket:
                case TokenType.CloseBracket:
                case TokenType.OpenArrayAccessor:
                case TokenType.CloseArrayAccessor:
                    return TokenCatagory.Punctuation;

                case TokenType.Addition:
                case TokenType.Subtract:
                case TokenType.Multiply:
                case TokenType.Divide:
                case TokenType.PlusPlus:
                case TokenType.MinusMinus:
                case TokenType.Modulo:
                    return TokenCatagory.Arithmetic;

                case TokenType.Assignment:
                case TokenType.AssignmentAdd:
                case TokenType.AssignmentDivide:
                case TokenType.AssignmentMultiply:
                case TokenType.AssignmentSubtract:
                    return TokenCatagory.Assignment;

                case TokenType.LogicalAnd:
                case TokenType.LogicalEquals:
                case TokenType.LogicalGreater:
                case TokenType.LogicalGreaterOrEqual:
                case TokenType.LogicalLess:
                case TokenType.LogicalLessOrEqual:
                case TokenType.LogicalNot:
                case TokenType.LogicalNotEquals:
                case TokenType.LogicalOr:
                    return TokenCatagory.Logical;

                case TokenType.BitwiseAnd:
                case TokenType.BitwiseOr:
                case TokenType.BitwiseXOR:
                    return TokenCatagory.Bitwise;

                default:
                    return TokenCatagory.None;
            }
        }

        public bool IsInvalid()
        {
            return Type == TokenType.Invalid;
        }

        public bool IsTrivia()
        {
            switch (Type)
            {
                case TokenType.WhiteSpace:
                case TokenType.Comment:
                case TokenType.NewLine:
                    return true;

                default: return false;
            }
        }

        #region Equality Operations

        public static bool operator !=(Token left, TokenType right)
        {
            return !(left == right);
        }

        public static bool operator !=(TokenType left, Token right)
        {
            return !(left == right);
        }

        public static bool operator !=(Token left, string right)
        {
            return !(left == right);
        }

        public static bool operator !=(string left, Token right)
        {
            return !(left == right);
        }

        public static bool operator ==(Token left, TokenType right)
        {
            return left?.Equals(right) ?? false;
        }

        public static bool operator ==(TokenType left, Token right)
        {
            return right?.Equals(left) ?? false;
        }

        public static bool operator ==(Token left, string right)
        {
            return left?.Equals(right) ?? false;
        }

        public static bool operator ==(string left, Token right)
        {
            return right?.Equals(left) ?? false;
        }

        public bool Equals(string other)
        {
            return Value == other;
        }

        public bool Equals(TokenType other)
        {
            return Type == other;
        }

        public override bool Equals(object obj)
        {
            if (obj is string)
            {
                return Equals((string)obj);
            }
            else if (obj is TokenType)
            {
                return Equals((TokenType)obj);
            }
            else
            {
                return base.Equals(obj);
            }
        }

        public override int GetHashCode()
        {
            return (Start.GetHashCode()
                  ^ End.GetHashCode()
                  ^ Type.GetHashCode()
                  ^ Value.GetHashCode());
        }

        #endregion Equality Operations
    }
}