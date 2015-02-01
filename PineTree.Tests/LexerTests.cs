using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PineTree.Language;
using PineTree.Language.Lexer;
using Xunit;

namespace PineTree.Tests
{
    public class LexerTests
    {
        private PineTreeLexer lexer = new PineTreeLexer();

        private IEnumerable<Token> Lex(string code)
        {
            lexer.Feed(code);
            return lexer.Lex().ToArray();
        }

        [Fact]
        private void LexerCreatesKeyword()
        {
            const string code = "public";
            var tokens = Lex(code);

            Assert.Contains(TokenType.Keyword, tokens.Select(g => g.Type));
        }

        [Fact]
        private void LexerTakesWhiteSpace()
        {
            const string code = "    \t    ";
            var tokens = Lex(code);

            Assert.Collection(tokens, (token => token.IsTrivia()), (token => token.Type.Equals(TokenType.WhiteSpace)));
        }

        private void LexerThrowsWithInfiniteString()
        {
            const string code = "\"";
            Assert.Throws<LexerException>(() => Lex(code));
        }

        [Theory]
        [InlineData("1f")]
        [InlineData("1.0")]
        [InlineData("1.0f")]
        [InlineData("1.99")]
        [InlineData("01.2f")]
        [InlineData("2e+2")]
        [InlineData("3e-8f")]
        private void LexesFloat(string code)
        {
            var token = Lex(code).First();
            Assert.Equal(TokenType.FloatingPointNumber, token.Type);
        }

        [Theory]
        [InlineData("_")]
        [InlineData("_3")]
        [InlineData("_____________________")]
        [InlineData("f")]
        [InlineData("f1")]
        [InlineData("_43ge_few1_321123")]
        [InlineData("publicprivate")]
        [InlineData("Public")]
        [InlineData("_protected")]
        private void LexesIdentifier(string code)
        {
            var token = Lex(code).First();

            Assert.Equal(TokenType.Identifier, token.Type);
        }

        [Theory]
        [InlineData("1293409827402347203", TokenType.Number)]
        [InlineData("1561065165065", TokenType.Number)]
        [InlineData("12131513", TokenType.Number)]
        [InlineData("0123456789", TokenType.Number)]
        [InlineData("0x0123456789abcdef", TokenType.HexNumber)]
        [InlineData("0xfedcba9876543210", TokenType.HexNumber)]
        [InlineData("0c01234567", TokenType.OctalNumber)]
        private void LexesNumbers(string code, TokenType type)
        {
            var token = Lex(code).First();
            Assert.Equal(type, token.Type);
        }

        [Theory]
        [InlineData("jfoiwejgoiw jiowejfoiwf jfwopiefjwof")]
        [InlineData(">>>><><><><><>>>><<<<>>>>////!!!!!!!![][][][[[[[]]][][]]][[][]\\\\\\\\\\\\\";;,,.,.,.,.")]
        [InlineData("1242 12421 0x3333333           ")]
        private void LexesToEOF(string code)
        {
            Assert.True(Lex(code).Last().Type == TokenType.EOF);
        }

        [Theory]
        [InlineData("1")]
        [InlineData("12")]
        [InlineData("public")]
        [InlineData("hello")]
        [InlineData("???")]
        [InlineData("true")]
        private void LexSingleToken(string code)
        {
            lexer.Feed(code);
            Assert.True(lexer.Size == code.Length);
            var token = lexer.LexToken(g => true);

            Assert.Equal(code, token.Value);
        }
    }
}