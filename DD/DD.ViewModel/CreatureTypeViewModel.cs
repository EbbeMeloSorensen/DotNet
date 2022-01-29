using Craft.ViewModel.Utils;
using DD.Domain;

namespace DD.ViewModel
{
    public class CreatureTypeViewModel : ListBoxItemViewModel<CreatureType>
    {
        public string DisplayText
        {
            get
            {
                return ((CreatureType)Object).Name;
            }
        }

        public CreatureTypeViewModel(CreatureType creatureType) : base(creatureType)
        {
        }
    }
}