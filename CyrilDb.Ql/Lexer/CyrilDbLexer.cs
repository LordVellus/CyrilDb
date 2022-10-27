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

    public class CyrilDbLexer
    {
        public CyrilDbLexerTokenResult Analyse(string inInput)
        {
            var result = new CyrilDbLexerTokenResult();

            for(var i = 0; i < inInput.Length; i++)
            {
                var character = inInput[i].ToString();

                if (character == "\r")
                {
                    var newLineCheck = LookAheadTo(inInput, i, Environment.NewLine);

                    if (newLineCheck == System.Environment.NewLine)
                    {
                        m_lineNumber++;
                        m_colNumber = 0;
                        i += Environment.NewLine.Length - 1;
                        continue;
                    }
                }

                if (string.IsNullOrWhiteSpace(character))
                {
                    continue;
                }

                var keyWordCheck = CheckForValidInput(inInput, i, LexerTokenType.KEYWORD, m_keywordDfa);

                if(ValidatePotentialInputResult(keyWordCheck, ref result, ref i))
                {
                    continue;
                }

                var identifierCheck = CheckForValidInput(inInput, i, LexerTokenType.IDENTIFIER, m_identifierDfa);

                if(ValidatePotentialInputResult(identifierCheck, ref result, ref i))
                {
                    continue;
                }

                var operatorCheck = CheckForValidInput(inInput, i, LexerTokenType.OPERATOR, m_operatorDfa);

                if(ValidatePotentialInputResult(operatorCheck, ref result, ref i))
                {
                    continue;
                }

                var numberCheck = CheckForValidInput(inInput, i, LexerTokenType.NUMBER, m_numberDfa);

                if(ValidatePotentialInputResult(numberCheck, ref result, ref i))
                {
                    continue;
                }

                var stringCheck = CheckForValidInput(inInput, i, LexerTokenType.STRING, m_stringDfa);

                if(ValidatePotentialInputResult(stringCheck, ref result, ref i))
                {
                    continue;
                }

                var specialCharacterCheck = m_specialCharacterDfa.Run(character);

                if(specialCharacterCheck.Success)
                {
                    result.Tokens.Add(new CyrilDbLexerToken
                    {
                        Token = LexerTokenConverter.ToEnum(specialCharacterCheck.Value),
                        TokenType = LexerTokenType.SPECIAL_CHARACTER,
                        TokenValue = specialCharacterCheck.Value
                    });

                    continue;
                }

                //  TODO : Track line and column numbers.
                throw new Exception($"Lexer failed at line: {m_lineNumber} column: {m_colNumber}");
            }

            return result;
        }

        private KeywordDfa m_keywordDfa = new KeywordDfa();
        private IdentifierDfa m_identifierDfa = new IdentifierDfa();
        private SpecialCharacterDfa m_specialCharacterDfa = new SpecialCharacterDfa();
        private OperatorDfa m_operatorDfa = new OperatorDfa();
        private NumberDfa m_numberDfa = new NumberDfa();
        private StringDfa m_stringDfa = new StringDfa();

        private int m_lineNumber = 0;
        private int m_colNumber = 0;

        private void UpdateColumnNumber(int inIndex)
        {
            m_colNumber = inIndex - m_colNumber;
        }

        private bool IsValidLexerToken(CyrilDbLexerToken inLexerToken)
        {
            return inLexerToken != null;
        }

        private class CheckResult
        {
            public bool Success = false;
            public CyrilDbLexerToken LexerToken = null;
            public int NextIndex = 0;
        }

        private bool ValidatePotentialInputResult(CheckResult inResult, ref CyrilDbLexerTokenResult inLexerTokenResult, ref int inIndex)
        {
            if (inResult.Success)
            {
                inIndex = inResult.NextIndex;
                inLexerTokenResult.Tokens.Add(inResult.LexerToken);
            }

            UpdateColumnNumber(inIndex);

            return inResult.Success;
        }

        private CheckResult CheckForValidInput(string inInput, int inIndex, LexerTokenType inTokenType, Dfa inDfa)
        {
            var result = new CheckResult();

            int nextIndex = inIndex;

            var potentialInput = "";

            if (inInput[nextIndex].ToString() == "\"")
            {
                potentialInput = LookAheadString(inInput, inIndex, out  nextIndex);
            }
            else
            {
                potentialInput = LookAhead(inInput, inIndex, out nextIndex);
            }

            var potentialInputResult = inDfa.Run(potentialInput);

            var potentialSuccess = potentialInputResult.Success;
            result.Success = potentialSuccess;

            var lexerToken = LexerTokenConverter.ToEnum(potentialInputResult.Value);

            var shouldCheckLexerToken = inTokenType == LexerTokenType.SPECIAL_CHARACTER 
                || inTokenType == LexerTokenType.OPERATOR 
                || inTokenType == LexerTokenType.KEYWORD;

            if(shouldCheckLexerToken)
            {
                result.Success = potentialSuccess && lexerToken != LexerToken.ERROR;
            }

            result.LexerToken = new CyrilDbLexerToken()
            {
                Token = lexerToken,
                TokenType = inTokenType,
                TokenValue = potentialInputResult.Value
            };

            if(result.Success)
            {
                result.NextIndex = nextIndex;
            }
            
            return result;
        }

        private bool IsWhiteSpace(string inInput, int inIndex)
        {
            if(inIndex >= inInput.Length)
            {
                return false;
            }

            return string.IsNullOrWhiteSpace(inInput[inIndex].ToString());
        }

        private string LookAhead(string inInput, int inIndex, out int lastIndex)
        {
            var validText = "";

            while (inIndex < inInput.Length && !IsWhiteSpace(inInput, inIndex))
            {
                validText += inInput[inIndex].ToString();

                if(IsWhiteSpace(inInput, inIndex + 1))
                {
                    break;
                }

                inIndex++;
            }

            lastIndex = inIndex;
            return validText;
        }

        private string LookAheadString(string inInput, int inIndex, out int lastIndex)
        {
            var validText = "";

            while(inIndex < inInput.Length)
            {
                validText += inInput[inIndex].ToString();
                inIndex++;

                if(inInput[inIndex].ToString() == "\"")
                {
                    validText += "\"";
                    break;
                }
            }

            lastIndex = inIndex;
            return validText;
        }

        private string LookAheadTo(string inInput, int inIndex, string inStringCheck)
        {
            var lookAheadText = inInput[inIndex].ToString();

            while(inIndex < inInput.Length && lookAheadText != inStringCheck)
            {
                inIndex++;
                lookAheadText += inInput[inIndex];
            }

            return lookAheadText;
        }
    }
}
