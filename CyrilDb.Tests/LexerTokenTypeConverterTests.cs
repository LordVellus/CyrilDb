using CyrilDb.Ql.Lexer;
using NUnit.Framework;

namespace CyrilDb.Tests
{
    public class LexerTokenTypeConverterTests
    {
        [Test]
        public void FromEnum()
        {
           var get = LexerTokenConverter.FromEnum(LexerToken.GET);
            Assert.AreEqual(get, "get");
        }

        [Test]
        public void ToEnum()
        {
            var put = LexerTokenConverter.ToEnum("put");
            Assert.AreEqual(put, LexerToken.PUT);
        }
    }
}
