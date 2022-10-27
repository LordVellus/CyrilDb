using CyrilDb.Ql;
using NUnit.Framework;
using System.Collections.Generic;
using CyrilDb.Ql.Fsm.DfaUtils;

namespace CyrilDb.Tests.Ql.Tests
{
    public class FsmIdentifiersTest
    {

        private Dfa m_dfa = new Dfa();
        
        [SetUp]
        public void Setup()
        {

            m_dfa.SetStartingState(StateId.A)
                .AddState(StateId.A, false, ch =>
                {
                    if (DfaStringUtils.IsLetter(ch) || ch == "_")
                    {
                        return StateId.B;
                    }

                    return StateId.None;
                })
                .AddState(StateId.B, true, ch =>
                {
                    if (DfaStringUtils.IsLetter(ch) || DfaStringUtils.IsDigit(ch) || ch == "_")
                    {
                        return StateId.B;
                    }

                    return StateId.None;
                });
        }

        [Test]
        public void Success()
        {
            var result = m_dfa.Run("camelCaseIdentifier");
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Value, "camelCaseIdentifier");

            result = m_dfa.Run("snake_case_identifier");
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Value, "snake_case_identifier");

            result = m_dfa.Run("_identifierStartingWithUnderscore");
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Value, "_identifierStartingWithUnderscore");

            result = m_dfa.Run("ident1f1er_cont4ining_d1g1ts");
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Value, "ident1f1er_cont4ining_d1g1ts");

        }

        [Test]
        public void Fail()
        {
            var result = m_dfa.Run("1dentifier_starting_with_digit");
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Value, "1");

            result = m_dfa.Run("lisp-case-identifier");
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Value, "lisp-");
        }
    }
}
