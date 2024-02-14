using System;
using System.Threading.Tasks;
using Craft.Logging;
using DD.Application.BattleEvents;

namespace DD.Application
{
    public class Application
    {
        private ILogger _logger;

        // It must be possible for an external component to set the Logger, e.g. in order to override with a decorator
        public ILogger Logger
        {
            get { return _logger;}
            set
            {
                _logger = value;

                if (Engine != null)
                {
                    Engine.Logger = value;
                }
            }
        }

        public IEngine Engine { get; set; }

        public Application(
            ILogger logger)
        {
            _logger = logger;
        }

        public async Task ActOutBattle()
        {
            if (Engine == null)
            {
                throw new InvalidOperationException("Engine not set");
            }

            await Task.Run(async () =>
            {
                Engine.StartBattle();
                Engine.StartBattleRound();

                while (true)
                {
                    if (Engine.BattleDecided)
                    {
                        break;
                    }

                    if (Engine.BattleroundCompleted)
                    {
                        Engine.StartBattleRound();
                    }

                    var nextEvent = await Engine.ExecuteNextEvent();

                    if (nextEvent is CreaturePass)
                    {
                        Engine.SwitchToNextCreature();
                    }
                }
            });
        }
    }
}
