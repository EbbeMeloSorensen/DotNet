using DD.Domain;

namespace DD.Persistence
{
    public static class SceneExtensions
    {
        public static Scene Clone(
            this Scene scene)
        {
            var clone = new Scene();

            clone.CopyAttributes(scene);
            return clone;
        }

        public static void CopyAttributes(
            this Scene scene,
            Scene other)
        {
            scene.Id = other.Id;
            scene.Name = other.Name;
            scene.Rows = other.Rows;
            scene.Columns = other.Columns;
        }
    }
}