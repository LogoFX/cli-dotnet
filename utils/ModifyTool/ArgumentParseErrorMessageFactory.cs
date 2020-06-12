namespace ModifyTool
{
    internal interface IArgumentParseErrorMessageFactory
    {
        string GetMessage(ArgumentParseErrorType errorType, string argument);
    }

    internal class ArgumentParseErrorMessageFactory : IArgumentParseErrorMessageFactory
    {
        public string GetMessage(ArgumentParseErrorType errorType, string argument)
        {
            return errorType switch
            {
                ArgumentParseErrorType.InvalidKey => $"{argument} is not a key",
                ArgumentParseErrorType.UnknownKey => $"Unknown key {argument}",
                ArgumentParseErrorType.NoKeysFound => "No keys found",
                ArgumentParseErrorType.InvalidNumberOfArguments => "Invalid number of arguments",
                _ => $"Unknown error with key {argument}"
            };
        }
    }
}