using CommandsShip;

namespace NavigatorShip
{
    public interface ICommandLogic
    {
        void ExecuteRequest(Command command);
    }
}