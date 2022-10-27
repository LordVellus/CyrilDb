using System;
using System.Collections.Generic;
using System.Text;

namespace CyrilDb.Ql.DfaMachines
{
    public class SpecialCharacterDfa : Dfa
    {
        public SpecialCharacterDfa()
        {
            this.SetStartingState(StateId.A)
                .AddState(StateId.A, false, ch =>
                {
                    foreach(var specialCharacter in m_specialCharacters)
                    {
                        if(specialCharacter == ch)
                        {
                            return StateId.B;
                        }
                    }

                    return StateId.None;
                })
                .AddState(StateId.B, true, ch =>
                {
                    return StateId.None;
                });
        }

        private List<string> m_specialCharacters = new List<string>
        {
            "(",
            ")",
        };
    }
}
