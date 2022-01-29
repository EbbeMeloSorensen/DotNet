using Craft.ViewModel.Utils;
using DD.Domain;

namespace DD.ViewModel
{
    public class SceneViewModel : ListBoxItemViewModel<Scene>
    {
        public string DisplayText
        {
            get
            {
                return ((Scene)Object).Name;
            }
        }

        public SceneViewModel(Scene scene) : base(scene)
        {
        }
    }
}
