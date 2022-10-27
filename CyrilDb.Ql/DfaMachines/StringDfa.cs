using CyrilDb.Ql.Fsm.DfaUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyrilDb.Ql.DfaMachines
{
    public class StringDfa : Dfa
    {
        public StringDfa()
        {
            this.SetStartingState(StateId.A)
                .AddState(StateId.A, false, ch =>
                {
                    if(ch == "\"")
                    {
                        return StateId.B;
                    }

                    return StateId.None;
                })
                .AddState(StateId.B, false, ch =>
                {
                    if(ch.IsLetter() || ch.IsDigit() || ch == "_" || ch == " ")
                    {
                        return StateId.B;
                    }

                    if(ch == "\"")
                    {
                        return StateId.C;
                    }

                    return StateId.None;
                })
                .AddState(StateId.C, true, ch =>
                {
                    return StateId.None;
                });
        }
    }
}
