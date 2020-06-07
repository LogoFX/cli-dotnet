namespace Common.Infra
{
    public static class StringExtensions
    {
        public static string ToCamelCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return str;
            }

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }
    }
}