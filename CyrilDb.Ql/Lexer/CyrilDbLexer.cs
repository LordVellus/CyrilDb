using CyrilDb.Ql.DfaMachines;
using CyrilDb.Ql.Fsm.DfaUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyrilDb.Ql.Lexer
{
    public class CyrilDbLexerToken
    {
        public LexerTokenType TokenType { get;set; }
        public LexerToken Token { get; set; }
        public string TokenValue {  get; set;}
    }

    public class CyrilDbLexerTokenResult
    {
        public List<CyrilDbLexerToken> Tokens = new List<CyrilDbLexerToken>();
    }

    public class AnalyseResult
    {
        public List<Token> Tokens { get; private set; } = new List<Token>();
        public List<string> Errors { get; private set; } = new List<string>();

        public void AddToken(Token inToken)
        {
            Tokens.Add(inToken);
        }

        public void AddError(string inError)
        {
            Errors.Add(inError);
        }
    }

    public class CyrilDbLexer
    {
        public AnalyseResult AnalyseResult { get;private set; } = new AnalyseResult();

        public CyrilDbLexer()
        {
        }
        
        public AnalyseResult Analyse(string inSource)
        {
            m_source = inSource;

            while (!IsAtEnd())
            {
                m_startPos = m_currentPos;
                ScanToken();
            }

            AnalyseResult.Tokens.Add(new Token(TokenType.EOF, "", m_line, m_column));

            return AnalyseResult;
        }

        private bool IsAtEnd()
        {
            return m_currentPos >= m_source.Length;
        }

        private bool PeekIsAtEnd(int pos)
        {
            return  pos >= m_source.Length;
        }

        private string Advance()
        {
            m_column++;
            return m_source[m_currentPos++].ToString();
        }

        private bool Match(string expected)
        {
            if(IsAtEnd()) 
            {
                return false;
            }

            if (m_source[m_currentPos].ToString() != expected)
            {
                return false;
            }

            Advance();
            return true;
        }

        private string Peek()
        {
            if (IsAtEnd())
            {
                return null;
            }

            return m_source[m_currentPos].ToString();
        }

        private bool IsNewLine(string c)
        {
            var newLineLength = Environment.NewLine.Length;

            if(newLineLength == 1)
            {
                return c == Environment.NewLine;
            }

            if (c != Environment.NewLine[0].ToString())
            {
                return false;
            }

            var newLineCheck = c;
            newLineCheck += Peek();

            return newLineCheck == Environment.NewLine;
        }

        private bool IsAlpha(string c)
        {
            return c.IsLetter() || c == "_";
        }

        private bool IsAlphaNumeric(string c)
        {
            return c.IsLetter() || c.IsDigit() || c == "_";
        }

        private void ScanToken()
        {
            var c = Advance();

            switch (c)
            {
                case "(":
                    AddToken(TokenType.LEFT_PAREN);
                    break;
                case ")":
                    AddToken(TokenType.RIGHT_PAREN);
                    break;
                case "{":
                    AddToken(TokenType.LEFT_BRACE);
                    break;
                case "}":
                    AddToken(TokenType.RIGHT_BRACE);
                    break;
                case ",":
                    AddToken(TokenType.COMMA);
                    break;
                case ".":
                    AddToken(TokenType.DOT);
                    break;
                case "-":
                    AddToken(TokenType.MINUS);
                    break;
                case "+":
                    AddToken(TokenType.PLUS);
                    break;
                case ";":
                    AddToken(TokenType.SEMICOLON);
                    break;
                case "*":
                    AddToken(TokenType.STAR);
                    break;

                case "!":
                    AddToken(Match("=") ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case "=":
                    AddToken(Match("=") ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case "<":
                    AddToken(Match("=") ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case ">":
                    AddToken(Match("=") ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case "&":
                    if(Match("&"))
                    {
                        AddToken(TokenType.AND);
                    }
                    break;
                case "|":
                    if(Match("|"))
                    {
                        AddToken(TokenType.OR);
                    }
                    break;

                case "/":

                    if(Match("/"))
                    {
                        while(!IsNewLine(Peek()) && !IsAtEnd())
                        {
                            Advance();
                        }
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }

                    break;

                case " ":
                case "\t":
                        break;

                case "\r":
                    if(Match("\n"))
                    {
                       IncrementLine();
                    }
                    break;
                case "\n":
                    IncrementLine();
                    break;

                case "\"":
                    ReadString();
                    break;

                default:

                    if(DfaStringUtils.IsDigit(c))
                    {
                        ReadNumber(c);
                    }
                    else if(c.IsLetter())
                    {
                        ReadIdentifier(c);
                    }
                    else
                    {
                        AnalyseResult.Errors.Add($"Unexpected character {c} line {m_line} column {m_column}");
                    }

                    break;
            }
        }

        private void AddToken(TokenType inTokenType)
        {
            var text = m_source.Substring(m_startPos, m_currentPos - m_startPos);
            AnalyseResult.AddToken(new Token(inTokenType, text, m_line, m_column));
        }

        private void AddToken(TokenType inTokenType, string inValue)
        {
            AnalyseResult.AddToken(new Token(inTokenType, inValue, m_line, m_column));
        }

        private void IncrementLine()
        {
            m_line++;
            m_column = 0;
        }

        private void ReadString()
        {
            var s = "";

            while(Peek() != "\"" && !IsAtEnd() && !IsNewLine(Peek()))
            {
                s += m_source[m_currentPos].ToString();
                Advance();
            }

            if(IsAtEnd() || m_source[m_currentPos].ToString() != "\"")
            {
                AnalyseResult.Errors.Add($"Unterminated string at line {m_line} column {m_column}");
                return;
            }

            Advance();

            AddToken(TokenType.STRING, s);
        }

        private void ReadNumber(string c)
        {
            var number = c;

            while (!string.IsNullOrWhiteSpace(Peek()))
            {
                number += m_source[m_currentPos];
                Advance();
            }

            var numberDfa = new NumberDfa();
            var numberResult = numberDfa.Run(number);

            if (numberResult.Success)
            {
                AddToken(TokenType.NUMBER, numberResult.Value);
            }
        }

        private void ReadIdentifier(string c)
        {
            var id = c;
            while (!string.IsNullOrWhiteSpace(Peek() ))
            {
                id += m_source[m_currentPos];
                Advance();
            }

            var keywordDfaResult = new KeywordDfa().Run(id);
            if(keywordDfaResult.Success && LexerTokenConverter<TokenType>.ToEnum(id) != TokenType.NOT_VALID)
            {
                AddToken(LexerTokenConverter<TokenType>.ToEnum(id), keywordDfaResult.Value);
                return;
            }
            
            var idDfaResult = new IdentifierDfa().Run(id);
            if(idDfaResult.Success)
            {
                AddToken(TokenType.IDENTIFIER, idDfaResult.Value);
            }
        }

        private string m_source = null;
        private int m_startPos = 0;
        private int m_currentPos = 0;

        private int m_line = 0;
        private int m_column = 0;

        //public CyrilDbLexerTokenResult Analyse(string inInput)
        //{
        //    var result = new CyrilDbLexerTokenResult();

        //    for(var i = 0; i < inInput.Length; i++)
        //    {
        //        var character = inInput[i].ToString();

        //        if (character == "\r")
        //        {
        //            var newLineCheck = LookAheadTo(inInput, i, Environment.NewLine);

        //            if (newLineCheck == System.Environment.NewLine)
        //            {
        //                m_lineNumber++;
        //                m_colNumber = 0;
        //                i += Environment.NewLine.Length - 1;
        //                continue;
        //            }
        //        }

        //        if (string.IsNullOrWhiteSpace(character))
        //        {
        //            continue;
        //        }

        //        var keyWordCheck = CheckForValidInput(inInput, i, LexerTokenType.KEYWORD, m_keywordDfa);

        //        if(ValidatePotentialInputResult(keyWordCheck, ref result, ref i))
        //        {
        //            continue;
        //        }

        //        var identifierCheck = CheckForValidInput(inInput, i, LexerTokenType.IDENTIFIER, m_identifierDfa);

        //        if(ValidatePotentialInputResult(identifierCheck, ref result, ref i))
        //        {
        //            continue;
        //        }

        //        var operatorCheck = CheckForValidInput(inInput, i, LexerTokenType.OPERATOR, m_operatorDfa);

        //        if(ValidatePotentialInputResult(operatorCheck, ref result, ref i))
        //        {
        //            continue;
        //        }

        //        var numberCheck = CheckForValidInput(inInput, i, LexerTokenType.NUMBER, m_numberDfa);

        //        if(ValidatePotentialInputResult(numberCheck, ref result, ref i))
        //        {
        //            continue;
        //        }

        //        var stringCheck = CheckForValidInput(inInput, i, LexerTokenType.STRING, m_stringDfa);

        //        if(ValidatePotentialInputResult(stringCheck, ref result, ref i))
        //        {
        //            continue;
        //        }

        //        var specialCharacterCheck = m_specialCharacterDfa.Run(character);

        //        if(specialCharacterCheck.Success)
        //        {
        //            result.Tokens.Add(new CyrilDbLexerToken
        //            {
        //                Token = LexerTokenConverter.ToEnum(specialCharacterCheck.Value),
        //                TokenType = LexerTokenType.SPECIAL_CHARACTER,
        //                TokenValue = specialCharacterCheck.Value
        //            });

        //            continue;
        //        }

        //        //  TODO : Track line and column numbers.
        //        throw new Exception($"Lexer failed at line: {m_lineNumber} column: {m_colNumber}");
        //    }

        //    return result;
        //}

        //private KeywordDfa m_keywordDfa = new KeywordDfa();
        //private IdentifierDfa m_identifierDfa = new IdentifierDfa();
        //private SpecialCharacterDfa m_specialCharacterDfa = new SpecialCharacterDfa();
        //private OperatorDfa m_operatorDfa = new OperatorDfa();
        //private NumberDfa m_numberDfa = new NumberDfa();
        //private StringDfa m_stringDfa = new StringDfa();

        //private int m_lineNumber = 0;
        //private int m_colNumber = 0;

        //private void UpdateColumnNumber(int inIndex)
        //{
        //    m_colNumber = inIndex - m_colNumber;
        //}

        //private bool IsValidLexerToken(CyrilDbLexerToken inLexerToken)
        //{
        //    return inLexerToken != null;
        //}

        //private class CheckResult
        //{
        //    public bool Success = false;
        //    public CyrilDbLexerToken LexerToken = null;
        //    public int NextIndex = 0;
        //}

        //private bool ValidatePotentialInputResult(CheckResult inResult, ref CyrilDbLexerTokenResult inLexerTokenResult, ref int inIndex)
        //{
        //    if (inResult.Success)
        //    {
        //        inIndex = inResult.NextIndex;
        //        inLexerTokenResult.Tokens.Add(inResult.LexerToken);
        //    }

        //    UpdateColumnNumber(inIndex);

        //    return inResult.Success;
        //}

        //private CheckResult CheckForValidInput(string inInput, int inIndex, LexerTokenType inTokenType, Dfa inDfa)
        //{
        //    var result = new CheckResult();

        //    int nextIndex = inIndex;

        //    var potentialInput = "";

        //    if (inInput[nextIndex].ToString() == "\"")
        //    {
        //        potentialInput = LookAheadString(inInput, inIndex, out  nextIndex);
        //    }
        //    else
        //    {
        //        potentialInput = LookAhead(inInput, inIndex, out nextIndex);
        //    }

        //    var potentialInputResult = inDfa.Run(potentialInput);

        //    var potentialSuccess = potentialInputResult.Success;
        //    result.Success = potentialSuccess;

        //    var lexerToken = LexerTokenConverter.ToEnum(potentialInputResult.Value);

        //    var shouldCheckLexerToken = inTokenType == LexerTokenType.SPECIAL_CHARACTER 
        //        || inTokenType == LexerTokenType.OPERATOR 
        //        || inTokenType == LexerTokenType.KEYWORD;

        //    if(shouldCheckLexerToken)
        //    {
        //        result.Success = potentialSuccess && lexerToken != LexerToken.ERROR;
        //    }

        //    result.LexerToken = new CyrilDbLexerToken()
        //    {
        //        Token = lexerToken,
        //        TokenType = inTokenType,
        //        TokenValue = potentialInputResult.Value
        //    };

        //    if(result.Success)
        //    {
        //        result.NextIndex = nextIndex;
        //    }
            
        //    return result;
        //}

        //private bool IsWhiteSpace(string inInput, int inIndex)
        //{
        //    if(inIndex >= inInput.Length)
        //    {
        //        return false;
        //    }

        //    return string.IsNullOrWhiteSpace(inInput[inIndex].ToString());
        //}

        //private string LookAhead(string inInput, int inIndex, out int lastIndex)
        //{
        //    var validText = "";

        //    while (inIndex < inInput.Length && !IsWhiteSpace(inInput, inIndex))
        //    {
        //        validText += inInput[inIndex].ToString();

        //        if(IsWhiteSpace(inInput, inIndex + 1))
        //        {
        //            break;
        //        }

        //        inIndex++;
        //    }

        //    lastIndex = inIndex;
        //    return validText;
        //}

        //private string LookAheadString(string inInput, int inIndex, out int lastIndex)
        //{
        //    var validText = "";

        //    while(inIndex < inInput.Length)
        //    {
        //        validText += inInput[inIndex].ToString();
        //        inIndex++;

        //        if(inInput[inIndex].ToString() == "\"")
        //        {
        //            validText += "\"";
        //            break;
        //        }
        //    }

        //    lastIndex = inIndex;
        //    return validText;
        //}

        //private string LookAheadTo(string inInput, int inIndex, string inStringCheck)
        //{
        //    var lookAheadText = inInput[inIndex].ToString();

        //    while(inIndex < inInput.Length && lookAheadText != inStringCheck)
        //    {
        //        inIndex++;
        //        lookAheadText += inInput[inIndex];
        //    }

        //    return lookAheadText;
        //}
    }
}
