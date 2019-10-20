using CADPadDB;
using CADPadDB.CADEntity;

namespace CADPadServices.ESelection
{
    internal abstract class EntityRS
    {
        internal abstract bool Cross(Bounding bounding, Entity entity);
        internal virtual bool Window(Bounding bounding, Entity entity)
        {
            return bounding.Contains(entity.Bounding);
        }
    }
}