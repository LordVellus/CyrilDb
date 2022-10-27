using CyrilDb.Ql.Lexer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyrilDb.Ql.DfaMachines
{
    public  class OperatorDfa : Dfa
    {
        public override LexerTokenType TokenType => LexerTokenType.OPERATOR;

        public List<LexerToken> Operators = new List<LexerToken>
        {
           LexerToken.OR,
           LexerToken.AND,
           LexerToken.NOT_EQUAL_TO,
           LexerToken.EQUAL_TO
        };

        public OperatorDfa()
        {
            this.SetStartingState(StateId.A)
                .AddState(StateId.A, true, ch =>
                {
                    PopulateValidInput();

                    foreach (var op in m_validInput)
                    {
                        if(op == ch)
                        {
                            return StateId.A;
                        }
                    }

                    return StateId.None;
                });
        }

        private List<string> m_validInput = new List<string>();

        private void PopulateValidInput()
        {
            if (m_validInput.Count == 0)
            {
                var allOperators = LexerTokenConverter.AllOperatorTokens();

                foreach (var op in allOperators)
                {
                    var opString = LexerTokenConverter.FromEnum(op);
                    var opArray = opString.ToCharArray();

                    foreach (var opA in opArray)
                    {
                        if (!m_validInput.Contains(opA.ToString()))
                        {
                            m_validInput.Add(opA.ToString());
                        }
                    }
                }
            }
        }
    }
}
