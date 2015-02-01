using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PineTree.Language.Lexer
{
    public class PineTreeLexer
    {
        private static List<string> _keywords = new List<string>()
        {
            "if",
            "else",
            "while",
            "for",

            "new",

            "class",

            "var",
            "void",

            "is",

            "import",

            "true",
            "false",

            "null",

            "public",
            "private",
            "protected",

            "static",

            "return",
        };

        private StringBuilder _builder;
        private int _index;
        private int _lineIndex;
        private int _lineNumber;
        private string _source;
        private Position _start;

        /// <summary>
        /// Gets the size of the currently loaded Source Code.
        /// </summary>
        public int Size => _source.Length;

        private char _ch => _source.CharAt(_index);

        private char _last => _source.CharAt(_index - 1);

        private char _next => _source.CharAt(_index + 1);

        /// <summary>
        /// Initializes a new instance of the <see cref="PineTreeLexer"/> with the given source code.
        /// </summary>
        /// <param name="sourceCode"></param>
        public PineTreeLexer(string sourceCode)
        {
            Reset(sourceCode);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PineTreeLexer"/> with no source code.
        /// </summary>
        public PineTreeLexer()
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Clears the current source code regardless of the progress of the lexing process.
        /// </summary>
        public void Clear()
        {
            Reset(string.Empty);
        }

        /// <summary>
        /// Feeds new source code into this <see cref="PineTreeLexer"/>.
        /// </summary>
        /// <remarks>
        /// If the lexer has reached the end of file, the cached source code will be removed.
        /// </remarks>
        /// <param name="sourceCode"></param>
        public void Feed(string sourceCode)
        {
            if (IsEOF())
            {
                Reset(sourceCode);
            }
            else
            {
                _source += sourceCode;
            }
        }

        /// <summary>
        /// Lexes the entire contents of the source code.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{Token}"/> which represents the contents of the provided source code.</returns>
        public IEnumerable<Token> Lex()
        {
            Token last;

            do
            {
                last = LexSingleToken();
                if (last.Type == TokenType.NewLine)
                {
                    _lineIndex = 0;
                    _lineNumber++;
                }
                yield return last;
            }
            while (last.Type != TokenType.EOF);
        }

        /// <summary>
        /// Replaces the current source code and lexes the contents.
        /// </summary>
        /// <seealso cref="Lex"/>
        /// <param name="sourceCode"></param>
        /// <returns></returns>
        public IEnumerable<Token> Lex(string sourceCode)
        {
            Reset(sourceCode);
            return Lex();
        }

        /// <summary>
        /// Lexes one token and checks it against the provided <see cref="Predicate{Token}"/>.  If the predicate returns <c>false</c>,
        /// the lexer will continue to lex until the predicate returns true.
        /// </summary>
        /// <param name="predicate">A <see cref="Predicate{Token}"/> that defines the conditions for a token to be returned.</param>
        /// <returns>The first token that matches <paramref name="predicate"/>.</returns>
        public Token LexToken(Predicate<Token> predicate)
        {
            if (IsEOF())
            {
                return CreateToken(TokenType.EOF);
            }
            Token token = LexSingleToken();

            if (!predicate(token))
            {
                return LexToken(predicate);
            }

            return token;
        }

        /// <summary>
        /// Peeks ahead a given number of tokens that match a given <see cref="Predicate{Token}"/>.
        /// </summary>
        /// <param name="ahead">The number of tokens to look ahead.</param>
        /// <param name="predicate">The predicate that defines the conditions for a token.</param>
        /// <returns>The token which is at the given index and matches the predicate.</returns>
        /// <seealso cref="PeekUntil(Predicate{Token})"/>
        public Token PeekToken(int ahead, Predicate<Token> predicate)
        {
            int index = _index;
            int line = _lineNumber;
            int lineIndex = _lineIndex;

            int count = 1;
            Token token;
            do
            {
                token = LexToken(predicate);
            } while (count++ < ahead);

            _index = index;
            _lineNumber = line;
            _lineIndex = lineIndex;

            return token;
        }

        /// <summary>
        /// Peeks ahead until a token that meets the given condition is found or the end of file is reached.
        /// </summary>
        /// <param name="predicate">The predicate that defines the conditions for a valid token.</param>
        /// <returns>An <see cref="IEnumerable{Token}"/> which represents the tokens that will be lexed which meet the given conditions.</returns>
        public IEnumerable<Token> PeekUntil(Predicate<Token> predicate)
        {
            int index = _index;
            int line = _lineNumber;
            int lineIndex = _lineIndex;

            List<Token> tokens = new List<Token>();

            bool end = false;

            while (!end && !IsEOF())
            {
                var token = LexSingleToken();
                if (predicate(token))
                {
                    end = true;
                }
                tokens.Add(token);
            }

            _index = index;
            _lineNumber = line;
            _lineIndex = lineIndex;

            return tokens.AsReadOnly();
        }

        /// <summary>
        /// Overwrites the current source code and resets all of the index pointers.
        /// </summary>
        /// <param name="sourceCode"></param>
        public void Reset(string sourceCode)
        {
            _source = sourceCode;
            _index = 0;
            _lineNumber = 1;
            _lineIndex = 0;
        }

        private Position CapturePosition()
        {
            _start = new Position(_lineNumber, _lineIndex, _index);

            return _start;
        }

        private Token CreateToken(TokenType type)
        {
            return new Token(_builder.ToString(), type, _start, CapturePosition());
        }

        private bool IsAlpha()
        {
            return char.IsLetter(_ch);
        }

        private bool IsAlphaNumeric()
        {
            return char.IsLetterOrDigit(_ch);
        }

        private bool IsDigit()
        {
            if (IsEOF())
            {
                return false;
            }

            return char.IsDigit(_ch);
        }

        private bool IsEOF()
        {
            return _ch == char.MinValue;
        }

        private bool IsHexDigit()
        {
            if (IsEOF())
            {
                return false;
            }

            return (_ch >= '0' && _ch <= '9')
                || (_ch >= 'a' && _ch <= 'f')
                || (_ch >= 'A' && _ch <= 'F');
        }

        private bool IsNewLine()
        {
            if (IsEOF())
            {
                return false;
            }

            return _ch == '\n';
        }

        private bool IsOctalDigit()
        {
            if (IsEOF())
            {
                return false;
            }

            var ch = _source.CharAt(_index);

            return (ch >= '0' && ch <= '7');
        }

        private bool IsReservedKeyword(string word)
        {
            return _keywords.Contains(word);
        }

        private bool IsSymbol()
        {
            if (IsEOF())
            {
                return false;
            }
            switch (_ch)
            {
                case '+':
                case '-':
                case '/':
                case '*':

                case '&':
                case '^':
                case '%':
                case '|':
                case '!':

                case '"':
                case '\'':

                case '=':

                case '(':
                case ')':
                case '{':
                case '}':
                case '[':
                case ']':
                case '<':
                case '>':

                case '?':
                case '.':
                case ',':
                case ':':
                case ';':
                    return true;

                default: return false;
            }
        }

        private bool IsWhiteSpace()
        {
            if (IsEOF())
            {
                return false;
            }

            return char.IsWhiteSpace(_ch) && _ch != '\n';
        }

        private Token LexSingleToken()
        {
            CapturePosition();
            _builder = new StringBuilder();

            if (IsEOF())
            {
                return new Token("_EOF_", TokenType.EOF, _start, _start);
            }
            else if (IsNewLine())
            {
                return ScanNewLine(true);
            }
            else if (IsWhiteSpace())
            {
                return ScanWhiteSpace();
            }
            else if (_ch == '/' && (_next == '/' || _next == '*'))
            {
                _index++;
                return ScanComment();
            }
            else if (IsAlpha() || _ch == '_')
            {
                return ScanIdentifier();
            }
            else if (_ch == '"')
            {
                _index++;
                return ScanStringLiteral();
            }
            else if (_ch == '0')
            {
                TakeChar();
                if (_ch == 'x')
                {
                    TakeChar();
                    return ScanHex();
                }
                else if (_ch == 'c')
                {
                    TakeChar();
                    return ScanOctal();
                }
                else
                {
                    return ScanNumber(false);
                }
            }
            else if (IsDigit())
            {
                return ScanNumber(false);
            }
            else if (IsSymbol())
            {
                return ScanOperator();
            }

            return ScanWord();
        }

        private Token ScanComment()
        {
            bool blockComment = false;
            if (_ch == '*')
            {
                blockComment = true;
            }

            _index++;

            while (!IsEOF() && !((blockComment && _last == '*' && _ch == '/') || (!blockComment && _ch == '\n')))
            {
                if (IsNewLine())
                {
                    ScanNewLine(false);
                }
                TakeChar();
            }

            if (blockComment)
            {
                _index++;
            }

            return CreateToken(TokenType.Comment);
        }

        private char ScanEscapeSequence()
        {
            switch (_ch)
            {
                case 'n':
                    return '\n';

                case 'r':
                    return '\r';

                case 't':
                    return '\t';

                case 'v':
                    return '\v';

                case '"':
                    return '"';

                case '\\':
                    return '\\';

                default:
                    return '\0';
            }
        }

        private Token ScanHex()
        {
            if (_ch == '-')
            {
                TakeChar();
            }

            while (IsHexDigit())
            {
                _builder.Append(_source.CharAt(_index++));
            }

            if (!IsEOF() && !IsSymbol() && !IsWhiteSpace())
            {
                return ScanWord();
            }

            return CreateToken(TokenType.HexNumber);
        }

        private Token ScanIdentifier()
        {
            while (IsAlphaNumeric() || _ch == '_')
            {
                TakeChar();
            }

            if (IsReservedKeyword(_builder.ToString()))
            {
                return new Token(_builder.ToString(), TokenType.Keyword, _start, CapturePosition());
            }

            return new Token(_builder.ToString(), TokenType.Identifier, _start, CapturePosition());
        }

        private Token ScanNewLine(bool createToken)
        {
            TakeChar();
            _lineIndex = 0;
            _lineNumber++;
            if (createToken)
            {
                return CreateToken(TokenType.NewLine);
            }
            return null;
        }

        private Token ScanNumber(bool inFloat)
        {
            if (_ch == '-')
            {
                TakeChar();
            }
            while (IsDigit())
            {
                TakeChar();
            }

            if (_ch == 'f')
            {
                _index++;
                return CreateToken(TokenType.FloatingPointNumber);
            }
            else if (!inFloat && _ch == '.')
            {
                TakeChar();
                return ScanNumber(true);
            }

            if ((!IsEOF() && !IsSymbol() && !IsWhiteSpace()) || _ch == '.')
            {
                return ScanWord();
            }

            if (inFloat)
            {
                return CreateToken(TokenType.FloatingPointNumber);
            }
            return CreateToken(TokenType.Number);
        }

        private Token ScanOctal()
        {
            if (_ch == '-')
            {
                TakeChar();
            }

            while (IsOctalDigit())
            {
                TakeChar();
            }

            if (!IsEOF() && !IsSymbol() && !IsWhiteSpace())
            {
                return ScanWord();
            }
            return CreateToken(TokenType.OctalNumber);
        }

        private Token ScanOperator()
        {
            switch (_ch)
            {
                case '|':
                    TakeChar();
                    if (_ch == '|')
                    {
                        TakeChar();
                        return CreateToken(TokenType.LogicalOr);
                    }
                    return CreateToken(TokenType.BitwiseOr);

                case '&':
                    TakeChar();
                    if (_ch == '&')
                    {
                        TakeChar();
                        return CreateToken(TokenType.LogicalAnd);
                    }
                    return CreateToken(TokenType.BitwiseAnd);

                case '=':
                    TakeChar();
                    if (_ch == '=')
                    {
                        TakeChar();
                        return CreateToken(TokenType.LogicalEquals);
                    }
                    else if (_ch == '>')
                    {
                        TakeChar();
                        return CreateToken(TokenType.FatArrow);
                    }
                    return CreateToken(TokenType.Assignment);

                case '!':
                    TakeChar();
                    if (_ch == '=')
                    {
                        TakeChar();
                        return CreateToken(TokenType.LogicalNotEquals);
                    }
                    return CreateToken(TokenType.LogicalNot);

                case '>':
                    TakeChar();
                    if (_ch == '=')
                    {
                        TakeChar();
                        return CreateToken(TokenType.LogicalGreaterOrEqual);
                    }
                    return CreateToken(TokenType.LogicalGreater);

                case '<':
                    TakeChar();
                    if (_ch == '=')
                    {
                        TakeChar();
                        return CreateToken(TokenType.LogicalLessOrEqual);
                    }
                    return CreateToken(TokenType.LogicalLess);

                case '*':
                    TakeChar();
                    if (_ch == '=')
                    {
                        TakeChar();
                        return CreateToken(TokenType.AssignmentMultiply);
                    }
                    return CreateToken(TokenType.Multiply);

                case '/':
                    TakeChar();
                    if (_ch == '=')
                    {
                        TakeChar();
                        return CreateToken(TokenType.AssignmentDivide);
                    }
                    return CreateToken(TokenType.Divide);

                case '+':
                    TakeChar();
                    if (_ch == '=')
                    {
                        TakeChar();
                        return CreateToken(TokenType.AssignmentAdd);
                    }
                    else if (_ch == '+')
                    {
                        TakeChar();
                        return CreateToken(TokenType.PlusPlus);
                    }
                    return CreateToken(TokenType.Addition);

                case '-':
                    TakeChar();
                    if (_ch == '=')
                    {
                        TakeChar();
                        return CreateToken(TokenType.AssignmentSubtract);
                    }
                    else if (_ch == '-')
                    {
                        TakeChar();
                        return CreateToken(TokenType.MinusMinus);
                    }
                    return CreateToken(TokenType.Subtract);

                case '%':
                    TakeChar();
                    return CreateToken(TokenType.Modulo);

                case '^':
                    TakeChar();
                    return CreateToken(TokenType.BitwiseXOR);

                case ':':
                    TakeChar();
                    return CreateToken(TokenType.Colon);

                default: return ScanPunctuation();
            }
        }

        private Token ScanPunctuation()
        {
            switch (_ch)
            {
                case ';':
                    TakeChar();
                    return CreateToken(TokenType.Semicolon);

                case '.':
                    TakeChar();
                    return CreateToken(TokenType.Dot);

                case ',':
                    TakeChar();
                    return CreateToken(TokenType.Comma);

                case '(':
                    TakeChar();
                    return CreateToken(TokenType.OpenPara);

                case ')':
                    TakeChar();
                    return CreateToken(TokenType.ClosePara);

                case '{':
                    TakeChar();
                    return CreateToken(TokenType.OpenBracket);

                case '}':
                    TakeChar();
                    return CreateToken(TokenType.CloseBracket);

                case '[':
                    TakeChar();
                    return CreateToken(TokenType.OpenArrayAccessor);

                case ']':
                    TakeChar();
                    return CreateToken(TokenType.CloseArrayAccessor);

                default: return ScanWord();
            }
        }

        private Token ScanStringLiteral()
        {
            while (_ch != '"')
            {
                if (IsEOF())
                {
                    throw new LexerException("Unexpected End of File.");
                }
                if (_ch == '\\')
                {
                    _index++;
                    char ch = ScanEscapeSequence();
                    _index++;

                    _builder.Append(ch);
                    continue;
                }
                TakeChar();
            }

            _index++;

            return CreateToken(TokenType.StringLiteral);
        }

        private Token ScanWhiteSpace()
        {
            while (IsWhiteSpace())
            {
                TakeChar();
            }

            return CreateToken(TokenType.WhiteSpace);
        }

        private Token ScanWord()
        {
            while (!IsEOF() && !IsWhiteSpace() && !IsSymbol())
            {
                TakeChar();
            }

            return CreateToken(TokenType.Invalid);
        }

        private void TakeChar()
        {
            _builder.Append(_ch);
            _lineIndex++;
            _index++;
        }
    }
}