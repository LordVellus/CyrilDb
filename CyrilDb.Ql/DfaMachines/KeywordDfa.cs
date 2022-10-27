using CyrilDb.Ql.Fsm.DfaUtils;
using CyrilDb.Ql.Lexer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyrilDb.Ql.DfaMachines
{
    public class KeywordDfa : Dfa
    {
        public override LexerTokenType TokenType => LexerTokenType.KEYWORD;
        public KeywordDfa()
        {
            this.SetStartingState(StateId.A)
                .AddState(StateId.A, true, ch =>
                {
                    if(DfaStringUtils.IsLetter(ch))
                    {
                        return StateId.A;
                    }

                    return StateId.None;
                });
        }
    }
}
