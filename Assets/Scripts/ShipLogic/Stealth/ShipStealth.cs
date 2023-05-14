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

        public override bool SeeOtherShip(ITargetToAttack ship)
        {
            var direction = ship.Position - Position;
            direction.y = 0.0f;
            var angleRotation = Vector3.Angle(direction, transform.forward);
            // todo константы в настройки
            // думаю эти значения тоже нужно подбирать с помощью алгоритм, так как они зависят на результат сражений
            // Debug.LogWarning($"AngleRotation: {angleRotation}. Distance: {Vector3.Distance(Position, ship.Position)}");
            return angleRotation <= 10f && Vector3.Distance(Position, ship.Position) <= Detector.Radius;
        }

        public override void PreparingForBattle(ITargetToAttack shipEnemy)
        {
            // При подготовке к бою нужно прилететь на ближайшее расстояние к цели
            // на минимальной скорости и крутиться пока не будет обнаружен враг
            Engine.SetTarget(shipEnemy.Position);
            Engine.TurnToTarget(shipEnemy.Position);
            Engine.Move();
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