using Players;
using Utils;

namespace SpaceObjects
{
    public interface IDetectedObject : IPosition
    {
        DetectedObjectType ObjectType { get; }
        PlayerType PlayerType { get; }
    }
}