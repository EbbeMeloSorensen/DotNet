namespace Simulator.Application
{
    public class ApplicationState
    {
        public string Name { get; private set; }

        public ApplicationState(
            string name)
        {
            Name = name;
        }
    }
}
