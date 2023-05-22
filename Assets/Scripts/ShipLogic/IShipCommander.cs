using System;
using SpaceObjects;
using StateMachineLogic;
using UnityEngine;

namespace ShipLogic
{
    public interface IShipCommander : IDisposable
    {
        StateBase Idle { get; }
        StateBase Attack { get; }
        StateBase Movement { get; }
        StateBase PrepareAttack { get; }
        bool HasEnemy { get; }
        bool HasPointForMovement { get; }
        bool IsNeedStop { get; }
        string NameCurrentState { get; }

        void SetPointForMovement(Vector3 pointPosition);
        
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