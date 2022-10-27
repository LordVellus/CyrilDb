using CyrilDb.Ql.Fsm.DfaUtils;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyrilDb.Ql.DfaMachines
{
    public class NumberDfa : Dfa
    {
        public NumberDfa()
        {
            this.SetStartingState(StateId.A)
                .AddState(StateId.A, true, ch =>
                {
                    if(ch.IsDigit())
                    {
                        return StateId.A;
                    }

                    if(ch == ".")
                    {
                        return StateId.B;
                    }

                    return StateId.None;
                })
                .AddState(StateId.B, true, ch =>
                {
                    return ch.IsDigit() ? StateId.B : StateId.None;
                });
        }
    }
}
