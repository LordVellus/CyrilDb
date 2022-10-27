using CyrilDb.Ql.Fsm.DfaUtils;

namespace CyrilDb.Ql.DfaMachines
{
    public class IdentifierDfa : Dfa
    {
        public IdentifierDfa()
        {
            this.SetStartingState(StateId.A)
                .AddState(StateId.A, true, ch =>
                {
                    if(ch.IsUnderscoreOrLetter())
                    {
                        return StateId.B;
                    }

                    return StateId.None;
                })
                .AddState(StateId.B, true, ch =>
                {
                    if (ch.IsDigit())
                    {
                        return StateId.B;
                    }

                    if(ch.IsUnderscoreOrLetter())
                    {
                        return StateId.B;
                    }

                    return StateId.None;
                });
        }
    }
}
