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

            Assert.IsTrue(tokenResult.Tokens.Count == 3);

            Assert.AreEqual(tokenResult.Tokens[0].TokenType, LexerTokenType.KEYWORD);
            Assert.AreEqual(tokenResult.Tokens[1].TokenType, LexerTokenType.KEYWORD);
            Assert.AreEqual(tokenResult.Tokens[2].TokenType, LexerTokenType.KEYWORD);

            Assert.AreEqual(tokenResult.Tokens[0].TokenValue, "GET");
            Assert.AreEqual(tokenResult.Tokens[1].TokenValue, "PUT");
            Assert.AreEqual(tokenResult.Tokens[2].TokenValue, "DELETE");
        }

        [Test]
        public void Strings()
        {
            var tokenResult = m_lexer.Analyse($"var string1 = \"this is string 1\"  {System.Environment.NewLine}var string2 = \"this is string 2\"{System.Environment.NewLine}        var string3 = \"this is string 3\"");

            Assert.AreEqual(tokenResult.Tokens[0].TokenValue, "var");
            Assert.AreEqual(tokenResult.Tokens[0].TokenType, LexerTokenType.KEYWORD);

            Assert.AreEqual(tokenResult.Tokens[1].TokenValue, "string1");
            Assert.AreEqual(tokenResult.Tokens[1].TokenType, LexerTokenType.IDENTIFIER);

            Assert.AreEqual(tokenResult.Tokens[3].TokenValue, "\"this is string 1\"");
            Assert.AreEqual(tokenResult.Tokens[3].TokenType, LexerTokenType.STRING);
        }

        [Test]
        public void Get()
        {
            var tokenResult = m_lexer.Analyse(@"GET
                                              table1");

            Assert.IsTrue(tokenResult.Tokens.Count == 2);

            Assert.AreEqual(tokenResult.Tokens[0].TokenType, LexerTokenType.KEYWORD);
            Assert.AreEqual(tokenResult.Tokens[1].TokenType, LexerTokenType.IDENTIFIER);

            Assert.AreEqual(tokenResult.Tokens[0].TokenValue, "GET");
            Assert.AreEqual(tokenResult.Tokens[1].TokenValue, "table1");
        }

        [Test]
        public void GetWhere()
        {
            var tokenResult = m_lexer.Analyse(@"GET
                table1      
                WHERE");

            Assert.IsTrue(tokenResult.Tokens.Count == 3);

            Assert.AreEqual(tokenResult.Tokens[0].TokenType, LexerTokenType.KEYWORD);
            Assert.AreEqual(tokenResult.Tokens[1].TokenType, LexerTokenType.IDENTIFIER);
            Assert.AreEqual(tokenResult.Tokens[2].TokenType, LexerTokenType.KEYWORD);

            Assert.AreEqual(tokenResult.Tokens[0].TokenValue, "GET");
            Assert.AreEqual(tokenResult.Tokens[1].TokenValue, "table1");
            Assert.AreEqual(tokenResult.Tokens[2].TokenValue, "WHERE");

        }

        [Test]
        public void WhereClause()
        {
            var tokenResult = m_lexer.Analyse(@"GET tabLe1 where (id == 3.999 && col1 == 500.1112 || col2 != 600023");

            Assert.IsTrue(tokenResult.Tokens.Count == 15);

            Assert.AreEqual(tokenResult.Tokens[14].TokenType, LexerTokenType.NUMBER);
            Assert.AreEqual(tokenResult.Tokens[2].TokenType, LexerTokenType.KEYWORD);
            Assert.AreEqual(tokenResult.Tokens[5].TokenType, LexerTokenType.OPERATOR);

            Assert.AreEqual(tokenResult.Tokens[14].TokenValue, "600023");
            Assert.AreEqual(tokenResult.Tokens[2].TokenValue, "where");
            Assert.AreEqual(tokenResult.Tokens[5].TokenValue, "==");
        }
    }
}
