using CyrilDb.Ql.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CyrilDb.Ql.Lexer
{
    public enum LexerTokenType
    {
        KEYWORD,
        NUMBER,
        STRING,
        OPERATOR,
        IDENTIFIER,
        SPECIAL_CHARACTER,
    }

    public enum LexerToken : uint
    {
        [LexerToken("ERROR")]
        ERROR,

        [LexerToken("get")]
        GET,
        [LexerToken("put")]
        PUT,
        [LexerToken("delete")]
        DELETE,
        [LexerToken("where")]
        WHERE,
        [LexerToken("var")]
        VAR,

        [LexerToken("||")]
        [OperatorToken]
        OR,
        [LexerToken("&&")]
        [OperatorToken]
        AND,
        [LexerToken("!=")]
        [OperatorToken]
        NOT_EQUAL_TO,
        [LexerToken("==")]
        [OperatorToken]
        EQUAL_TO,
        [LexerToken("=")]
        [OperatorToken]
        EQUALS,

        [LexerToken("(")]
        LEFT_PARANTHESIS,
        [LexerToken(")")]
        RIGHT_PARANTHESIS
    }

    public class LexerTokenConverter
    {
        public static string FromEnum(LexerToken inLexerToken)
        {
            FieldInfo fi = inLexerToken.GetType().GetField(inLexerToken.ToString());

            var attributes = fi.GetCustomAttributes(typeof(LexerTokenAttribute), false) as LexerTokenAttribute[];

            if(attributes != null && attributes.Any())
            {
                return attributes.First().StringRepresentation;
            }

            return null;
        }

        public static LexerToken ToEnum(string lexerToken)
        {
            var fields = typeof(LexerToken).GetFields();

            foreach(var field in fields)
            {
                var attributes = field.GetCustomAttributes(typeof(LexerTokenAttribute), false) as LexerTokenAttribute[];

                if (attributes != null && attributes.Any())
                {
                    if(attributes.First().StringRepresentation.Equals(lexerToken, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (LexerToken)field.GetValue(null);
                    }
                }
            }

            return LexerToken.ERROR;
        }

        public static List<LexerToken> AllOperatorTokens()
        {
            var lexerTokens = new List<LexerToken>();

           var fields = typeof(LexerToken).GetFields();

            foreach (var field in fields)
            {
                var attributes = field.GetCustomAttributes(typeof(OperatorTokenAttribute), false) as OperatorTokenAttribute[];

                if(attributes != null && attributes.Any())
                {
                    lexerTokens.Add((LexerToken)field.GetValue(null));
                }
            }

            return lexerTokens;
        }
    }
}
