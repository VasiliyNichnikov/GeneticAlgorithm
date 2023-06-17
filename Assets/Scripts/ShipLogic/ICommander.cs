using System;
using System.Collections.Generic;
using Map;
using Planets;
using SpaceObjects;
using StateMachineLogic;
using UnityEngine;

namespace ShipLogic
{
    public interface ICommander : ICommanderInfo, IDisposable
    {
        StateBase Idle { get; }
        StateBase Attack { get; }
        StateBase Movement { get; }
        StateBase PrepareAttack { get; }
        StateBase EscapeFromBattle { get; }
        bool HasEnemy { get; }
        bool HasPointForMovement { get; }
        bool IsNeedStop { get; }
        string NameCurrentState { get; }
        
        ShipType ShipType { get; }

        bool NeedEscapeFromBattle();
        bool IsDistanceToAttack();
        bool SeeOtherEnemyShip();
        bool CanAttackOtherEnemyShip();

        void TurnOnEngine();
        void TurnOffEngine();
        void MoveToSelectedPoint();
        void TurnToEnemyShip();
        void StartShoot();
        void ShootInEnemy();
        void FinishShoot();

        void ChangeStateToIdle();
        
        void SetPointForMovement(ITarget target);
        void SetPointForMovementToEnemy(ITarget target);
        void SetPointForEscapeFromBattle(ITarget target);

#if UNITY_EDITOR
        Vector3 GetPointForMovement();
#endif
    }
}