namespace CyrilDb.Ql.Fsm.DfaUtils
{
    public static class DfaStringUtils
    {
        public static bool IsLetter(this string s)
        {
            var ch = System.Convert.ToChar(s);
            return char.IsLetter(ch);
        }

        public static bool IsDigit(this string s)
        {
            var ch = System.Convert.ToChar(s);
            return char.IsDigit(ch);
        }

        public static bool IsUnderscore(this string s)
        {
            return s == "_";
        }

        public static bool IsUnderscoreOrLetter(this string s)
        {
            return IsUnderscore(s) || IsLetter(s);
        }
    }
}
