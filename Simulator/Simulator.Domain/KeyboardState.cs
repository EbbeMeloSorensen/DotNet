namespace Simulator.Domain
{
    public class Keyboard
    {
        public bool LeftArrowDown { get; set; }
        public bool RightArrowDown { get; set; }
        public bool UpArrowDown { get; set; }
        public bool DownArrowDown { get; set; }
        public bool SpaceDown { get; set; }

        public void Clear()
        {
            LeftArrowDown = false;
            RightArrowDown = false;
            UpArrowDown = false;
            DownArrowDown = false;
            SpaceDown = false;
        }

        public Keyboard Clone()
        {
            return new Keyboard
            {
                LeftArrowDown = LeftArrowDown,
                RightArrowDown = RightArrowDown,
                UpArrowDown = UpArrowDown,
                DownArrowDown = DownArrowDown,
                SpaceDown = SpaceDown
            };
        }
    }
}