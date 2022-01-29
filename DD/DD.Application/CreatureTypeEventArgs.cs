using System;
using DD.Domain;

namespace DD.Application
{
    public class CreatureTypeEventArgs : EventArgs
    {
        public readonly CreatureType CreatureType;

        public CreatureTypeEventArgs(
            CreatureType creatureType)
        {
            CreatureType = creatureType;
        }
    }
}
