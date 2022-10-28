using CyrilDb.Ql.Lexer;
using NUnit.Framework;

namespace CyrilDb.Tests
{
    public class LexerTokenTypeConverterTests
    {
        [Test]
        public void FromEnum()
        {
           var get = LexerTokenConverter<TokenType>.FromEnum(TokenType.GET);
            Assert.AreEqual(get, "get");
        }

        [Test]
        public void ToEnum()
        {
            var put = LexerTokenConverter<TokenType>.ToEnum("put");
            Assert.AreEqual(put, TokenType.PUT);
        }
    }
}
