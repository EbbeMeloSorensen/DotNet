using Games.Pig.Application;

namespace Games.Pig.ViewModel
{
    public class MainWindowViewModel
    {
        private Engine _engine;

        public MainWindowViewModel()
        {
            var players = new[] { true, false };
            _engine = new Engine(players);
        }
    }
}
