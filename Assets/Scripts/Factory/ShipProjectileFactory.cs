using ProjectileLogic;
using ShipLogic;
using ShipLogic.Stealth;
using UnityEngine;

namespace Factory
{
    public class ShipProjectileFactory : MonoBehaviour
    {
        [SerializeField] private ShipProjectile _projectilePrefab;
        [SerializeField] private Transform _parentProjectiles;
        
        public ShipProjectile CreateProjectile(Vector3 startPosition, Vector3 targetPosition, PlayerType playerType, float speed, float damage)
        {
            // todo нужно будет добавить pool
            var direction = targetPosition - startPosition;
            direction.y = 0.0f;

            var newProjectile = Instantiate(_projectilePrefab, _parentProjectiles, false);

            newProjectile.transform.position = startPosition;
            var newDirection = Vector3.RotateTowards(newProjectile.transform.right, direction, 360.0f, 0.0f);
            newProjectile.transform.rotation = Quaternion.LookRotation(newDirection, Vector3.right);

            newProjectile.Init(speed, damage, playerType);
            return newProjectile;
        }
    }
}