namespace ModifyTool
{
    internal interface IArgumentParseErrorHandler
    {
        void Handle(ArgumentParseErrorType errorType, string argument);
    }

    internal class ArgumentParseErrorHandler : IArgumentParseErrorHandler
    {
        private readonly IArgumentParseErrorMessageFactory _errorMessageFactory;
        private readonly IErrorMessageRenderer _errorMessageRenderer;

        public ArgumentParseErrorHandler(
            IArgumentParseErrorMessageFactory errorMessageFactory,
            IErrorMessageRenderer errorMessageRenderer)
        {
            _errorMessageFactory = errorMessageFactory;
            _errorMessageRenderer = errorMessageRenderer;
        }

        public void Handle(ArgumentParseErrorType errorType, string argument)
        {
            var errorMessage = _errorMessageFactory.GetMessage(errorType, argument);
            _errorMessageRenderer.RenderError(errorMessage);
        }
    }
}
