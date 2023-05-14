using JetBrains.Annotations;
using SpaceObjects;
using StateMachineLogic;
using UnityEngine;

namespace ShipLogic
{
    public interface IShipCommander
    {
        StateBase Idle { get; }
        StateBase Attack { get; }
        StateBase Movement { get; }
        StateBase PrepareAttack { get; }
        
        Vector3? PointForMovement { get; }
        [CanBeNull] ITargetToAttack Enemy { get; } 
        void SendDetectedObject(IDetectedObject detectedObject);
    }
}