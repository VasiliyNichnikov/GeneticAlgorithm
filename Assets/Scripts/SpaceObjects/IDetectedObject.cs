using Players;

namespace SpaceObjects
{
    public interface IDetectedObject
    {
        DetectedObjectType ObjectType { get; }
        PlayerType PlayerType { get; }
    }
}