using Craft.Utils;
using DD.Domain;
using DD.Application;

namespace DD.ViewModel
{
    /// <summary>
    /// Owns the view model collections for the obstacles and creatures on the board
    /// </summary>
    public class BoardViewModel : BoardViewModelBase
    {
        public BoardViewModel(
            IEngine engine, 
            double squareLength, 
            double obstacleDiameter, 
            double creatureDiameter, 
            double weaponDiameter, 
            ObservableObject<Scene> selectedScene) : base(
                engine, 
                squareLength, 
                obstacleDiameter, 
                creatureDiameter, 
                weaponDiameter, 
                selectedScene)
        {
        }
    }
}
