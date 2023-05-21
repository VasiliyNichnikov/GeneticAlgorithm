using SpaceObjects;
using StateMachineLogic;

namespace ShipLogic
{
    public interface IShipCommander
    {
        StateBase Idle { get; }
        StateBase Attack { get; }
        StateBase Movement { get; }
        StateBase PrepareAttack { get; }
        bool HasEnemy { get; }
        bool HasPointForMovement { get; }
        bool IsNeedStop { get; }
        string NameCurrentState { get; }

        void AddFoundEnemy(IDetectedObject detectedObject);
        void RemoveFoundEnemy(IDetectedObject detectedObject);
        

        bool SeeOtherEnemyShip();
        bool CanAttackOtherEnemyShip();
        
        void TurnOnEngine();
        void TurnOffEngine();
        void DestroyShip();
        void MoveToSelectedPoint();
        void TurnToEnemyShip();
        void StartShoot();
        void ShootInEnemy();
        void FinishShoot();
    }
}