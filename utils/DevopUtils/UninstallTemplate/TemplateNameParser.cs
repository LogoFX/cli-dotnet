namespace UninstallTemplate
{
    internal static class TemplateNameParser
    {
        internal enum TemplateNameKind
        {
            None,
            Name,
            ShortName,
            Directory
        }

        internal static TemplateNameKind GetTemplateNameKind(string str)
        {
            if (str.Length != 2)
            {
                return TemplateNameKind.None;
            }

            if (str[0] != '-')
            {
                return TemplateNameKind.None;
            }

            switch (str[1])
            {
                case 'n':
                case 'N':
                    return TemplateNameKind.Name;
                case 's':
                case 'S':
                    return TemplateNameKind.ShortName;
                case 'd':
                case 'D':
                    return TemplateNameKind.Directory;
                default:
                    return TemplateNameKind.None;
            }
        }
    }
}
