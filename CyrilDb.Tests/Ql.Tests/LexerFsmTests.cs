using NUnit.Framework;
using System.Collections.Generic;

namespace CyrilDb.Ql.Tests
{
    public class LexerFsmTests
    {
        private Dfa m_dfa = new Dfa();

        [SetUp]
        public void Setup()
        {
            m_dfa.SetStartingState(StateId.A)
                .AddState(StateId.A, false, 
                ch =>
                {
                    if (ch == "a" || ch == "b")
                    {
                        return StateId.B;
                    }

                    return StateId.None;
                })
                .AddState(StateId.B, false, 
                ch =>
                {
                    if(ch == "c")
                    {
                        return StateId.C;
                    }

                    return StateId.None;
                })
                .AddState(StateId.C, true, ch => { return StateId.None; });
        }

        [Test]
        public void A_or_B_and_C_Success()
        {            
            var result = m_dfa.Run("ac");
            Assert.IsTrue(result.Success);
            Assert.AreEqual("ac", result.Value);

            result = m_dfa.Run("bc");
            Assert.IsTrue(result.Success);
            Assert.AreEqual("bc", result.Value);
        }

        [Test]
        public void A_or_B_and_C_Fail()
        {
            var result = m_dfa.Run("abc");
            Assert.IsFalse(result.Success);
            Assert.AreEqual("ab", result.Value);
        }
    }
}