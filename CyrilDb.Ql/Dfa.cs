using CyrilDb.Ql.Lexer;
using System;
using System.Collections.Generic;
using System.Text;

namespace CyrilDb.Ql
{
    public enum StateId
    {
        None, A, B, C, D, E, F, G, H
    }
    public class Dfa
    {
        public const StateId NoNextState = StateId.None;
        public virtual LexerTokenType TokenType { get; private set;}

        public Dfa()
        {
            m_states.Add(StateId.None, false);
        }

        public Dfa SetStartingState(StateId inStartingState)
        {
            m_currentState = inStartingState;
            m_initialState = m_currentState;
            return this;
        }

        public Dfa AddState(StateId inStateId, bool inIsAcceptingState, Func<string, StateId> inNextStateTransition)
        {
            if(!m_states.ContainsKey(inStateId))
            {
                m_states.Add(inStateId, inIsAcceptingState);
            }
            
            if(!m_stateTransitions.ContainsKey(inStateId))
            {
                m_stateTransitions.Add(inStateId, inNextStateTransition);
            }
            
            return this;
        }

        public DfaResult Run(string input)
        {
            var result = new DfaResult();

            StateId nextState = StateId.None;
            m_currentState = m_initialState;

            for (var i = 0; i < input.Length; i++)
            {
                var character = input[i].ToString();
                result.Value += character;

                nextState = m_stateTransitions[m_currentState].Invoke(character);

                if(nextState == StateId.None)
                {
                    break;
                }

                m_currentState = nextState;
            }

            result.Success = m_states[nextState];
            return result;
        }

        private Dictionary<StateId, bool> m_states = new Dictionary<StateId, bool>();
        private Dictionary<StateId, Func<string, StateId>> m_stateTransitions = new Dictionary<StateId, Func<string, StateId>>();
        private StateId m_currentState = StateId.None;
        private StateId m_initialState = StateId.None;
    }

    public class DfaResult
    {
        public bool Success = true;
        public string Value;
    }
}
