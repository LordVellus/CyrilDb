using System;
using System.Collections.Generic;
using System.Text;

namespace CyrilDb.Ql.Attributes
{
    [AttributeUsage(AttributeTargets.All)]
    public class LexerTokenAttribute : Attribute
    {
        public string StringRepresentation { get; private set;}
        public LexerTokenAttribute(string inStringRepresentation)
        {
            StringRepresentation = inStringRepresentation;
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    public class OperatorTokenAttribute : Attribute
    {
    }
}
