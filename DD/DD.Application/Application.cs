namespace DD.Application
{
    public class Application
    {
        private IUIDataProvider _uiDataProvider;

        public IUIDataProvider UIDataProvider => _uiDataProvider;

        public Application(
            IUIDataProvider uiDataProvider)
        {
            _uiDataProvider = uiDataProvider;
        }
    }
}
