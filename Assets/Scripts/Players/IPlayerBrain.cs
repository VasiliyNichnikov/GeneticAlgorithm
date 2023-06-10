using Planets;

namespace Players
{
    public interface IPlayerBrain
    {
        ITarget GetTargetForMovement();
    }
}