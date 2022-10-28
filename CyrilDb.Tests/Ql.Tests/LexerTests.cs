using NUnit.Framework;
using CyrilDb.Ql.Lexer;

namespace CyrilDb.Tests.Ql.Tests
{
    public class LexerTests
    {
        private CyrilDbLexer m_lexer = new CyrilDbLexer();

        [SetUp]
        public void SetUp()
        {
            m_lexer = new CyrilDbLexer();
        }

        [Test]
        public void KeyWords()
        {
            var tokenResult = m_lexer.Analyse($"GET PUT DELETE");

            Assert.AreEqual(tokenResult.Tokens[0].Type, TokenType.GET);
            Assert.AreEqual(tokenResult.Tokens[1].Type, TokenType.PUT);
            Assert.AreEqual(tokenResult.Tokens[2].Type, TokenType.DELETE);

            Assert.AreEqual(tokenResult.Tokens[0].Lexeme, "GET");
            Assert.AreEqual(tokenResult.Tokens[1].Lexeme, "PUT");
            Assert.AreEqual(tokenResult.Tokens[2].Lexeme, "DELETE");
        }

        [Test]
        public void Strings()
        {
            var tokenResult = m_lexer.Analyse($"var string1 = \"this is string 1\"  {System.Environment.NewLine}var string2 = \"this is string 2\"{System.Environment.NewLine}        var string3 = \"this is string 3\"");

            Assert.AreEqual(tokenResult.Tokens[0].Lexeme, "var");
            Assert.AreEqual(tokenResult.Tokens[0].Type, TokenType.VAR);

            Assert.AreEqual(tokenResult.Tokens[1].Lexeme, "string1");
            Assert.AreEqual(tokenResult.Tokens[1].Type, TokenType.IDENTIFIER);

            Assert.AreEqual(tokenResult.Tokens[3].Lexeme, "this is string 1");
            Assert.AreEqual(tokenResult.Tokens[3].Type, TokenType.STRING);
        }

        [Test]
        public void Get()
        {
            var tokenResult = m_lexer.Analyse(@"GET
                                              table1");

            Assert.AreEqual(tokenResult.Tokens[0].Type, TokenType.GET);
            Assert.AreEqual(tokenResult.Tokens[1].Type, TokenType.IDENTIFIER);

            Assert.AreEqual(tokenResult.Tokens[0].Lexeme, "GET");
            Assert.AreEqual(tokenResult.Tokens[1].Lexeme, "table1");
        }

        [Test]
        public void GetWhere()
        {
            var tokenResult = m_lexer.Analyse(@"GET
                table1      
                WHERE");

            Assert.AreEqual(tokenResult.Tokens[0].Type, TokenType.GET);
            Assert.AreEqual(tokenResult.Tokens[1].Type, TokenType.IDENTIFIER);
            Assert.AreEqual(tokenResult.Tokens[2].Type, TokenType.WHERE);

            Assert.AreEqual(tokenResult.Tokens[0].Lexeme, "GET");
            Assert.AreEqual(tokenResult.Tokens[1].Lexeme, "table1");
            Assert.AreEqual(tokenResult.Tokens[2].Lexeme, "WHERE");

        }

        [Test]
        public void WhereClause()
        {
            var tokenResult = m_lexer.Analyse(@"GET tabLe1 where (id == 3.999 && col1 == 500.1112 || col2 != 600023");

            Assert.AreEqual(tokenResult.Errors.Count, 0);

            Assert.AreEqual(tokenResult.Tokens[14].Type, TokenType.NUMBER);
            Assert.AreEqual(tokenResult.Tokens[2].Type, TokenType.WHERE);
            Assert.AreEqual(tokenResult.Tokens[5].Type, TokenType.EQUAL_EQUAL);

            Assert.AreEqual(tokenResult.Tokens[14].Lexeme, "600023");
            Assert.AreEqual(tokenResult.Tokens[2].Lexeme, "where");
            Assert.AreEqual(tokenResult.Tokens[5].Lexeme, "==");
        }
    }
}
