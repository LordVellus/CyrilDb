using CyrilDb.Ql.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CyrilDb.Ql.Lexer
{
    public enum TokenType : uint
    {
        NOT_VALID,

        //  single character tokens
        LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
        COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

        //  one or two character tokens
        BANG, BANG_EQUAL,
        EQUAL, EQUAL_EQUAL,
        GREATER, GREATER_EQUAL,
        LESS, LESS_EQUAL, AND, OR,

        //  literals
        IDENTIFIER, STRING, NUMBER,

        //   keywords
        [LexerToken("var")]
        VAR,
        [LexerToken("get")]
        GET,
        [LexerToken("put")]
        PUT,
        [LexerToken("delete")]
        DELETE,
        [LexerToken("where")]
        WHERE,

        EOF
    }

    public class Token
    {
        public TokenType Type;
        public string Lexeme;
        public int Line;
        public int Column;

        public Token(TokenType inType, string inLexeme, int inLine, int inColumn)
        {
            Type = inType;
            Lexeme = inLexeme;
            Line = inLine;
            Column = inColumn;
        }
    }

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

    public class LexerTokenConverter<T> where T : System.Enum
    {
        public static string FromEnum(T inLexerToken)
        {
            FieldInfo fi = inLexerToken.GetType().GetField(inLexerToken.ToString());

            var attributes = fi.GetCustomAttributes(typeof(LexerTokenAttribute), false) as LexerTokenAttribute[];

            if(attributes != null && attributes.Any())
            {
                return attributes.First().StringRepresentation;
            }

            return null;
        }

        public static T ToEnum(string lexerToken)
        {
            var fields = typeof(T).GetFields();

            foreach(var field in fields)
            {
                var attributes = field.GetCustomAttributes(typeof(LexerTokenAttribute), false) as LexerTokenAttribute[];

                if (attributes != null && attributes.Any())
                {
                    if(attributes.First().StringRepresentation.Equals(lexerToken, System.StringComparison.InvariantCultureIgnoreCase))
                    {
                        return (T)field.GetValue(null);
                    }
                }
            }

            return default(T);
        }
    }
}
