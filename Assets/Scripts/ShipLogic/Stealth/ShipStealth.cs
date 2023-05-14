using System;
using ShipLogic.Stealth.States;
using StateMachineLogic;
using UnityEngine;

namespace ShipLogic.Stealth
{
    public class ShipStealth : ShipBase
    {

        protected override float OnBoostSpeed(float currentSpeed)
        {
            if (currentSpeed >= MaximumSpeed)
            {
                return MaximumSpeed;
            }

            var speedStep = GetSpeedStep();
            currentSpeed += speedStep * Time.deltaTime;
            return currentSpeed;
        }

        protected override float OnSlowingDownSpeed(float currentSpeed)
        {
            if (currentSpeed <= MinimumSpeed)
            {
                return MinimumSpeed;
            }

            var speedStep = GetSpeedStep();

            currentSpeed -= speedStep * Time.deltaTime;
            return currentSpeed;
        }

        public override void PreparingForBattle(ITargetToAttack shipEnemy, out bool stack)
        {
            Engine.SetTarget(shipEnemy.Position);
            Engine.TurnToTarget(shipEnemy.Position);
            stack = !Engine.Move();
        }

        private float GetSpeedStep()
        {
            if (MaximumSpeed > Weight)
            {
                Debug.LogError("Weight more then maximum speed");
                return 0.1f;
            }

            var dependence = MaximumSpeed / Weight + 1;
            var result = Mathf.Log(dependence) * 3.31f;
            result = Mathf.Clamp(result, 0.001f, 1.0f);
            return result;
        }
    }
}