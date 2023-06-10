using System;
using UnityEngine;

namespace ShipLogic
{
    public interface IShipHealth
    {
        event Action<float> OnUpdateHealth;
        event Action<float> OnUpdateArmor;
        
        float CurrentHealthPercentages { get; }
        float CurrentArmorPercentages { get; }
        
        string HealthStats { get; }
        string ArmorStats { get; }
        bool IsDead { get; }
        bool IsArmorDestroyed { get; }
        void DealDamage(float damage);
    }
    
    public class ShipHealth : IShipHealth
    {
        public event Action<float> OnUpdateHealth;
        public event Action<float> OnUpdateArmor;
        public float CurrentHealthPercentages => (_currentHealth - _minHealth) / (_maxHealth - _minHealth);
        public float CurrentArmorPercentages => (_currentArmor - _minArmor) / (_maxArmor - _minArmor);
        public string HealthStats => $"{_currentHealth}/{_maxHealth}";
        public string ArmorStats => $"{_currentArmor}/{_maxArmor}";
        public float MaxHealth => _maxHealth;
        public bool IsDead => _currentHealth <= _minHealth;
        public bool IsArmorDestroyed => _currentArmor <= _minArmor;

        #region Health

        private readonly float _maxHealth;
        private readonly float _minHealth;
        private float _currentHealth;

        #endregion

        #region Armor

        private readonly float _maxArmor;
        private readonly float _minArmor;
        /// <summary>
        /// Процент поглощения брони
        /// (Сколько урона броня может поглатить)
        /// Значение должно быть от 0 до 100
        /// </summary>
        private readonly float _percentageOfArmorAbsorption;
        private float _currentArmor;

        #endregion

        public ShipHealth(float minHealth, float maxHealth, float minArmor, float maxArmor,
            float percentageOfArmorAbsorption)
        {
            _minHealth = minHealth;
            _maxHealth = maxHealth;
            _minArmor = minArmor;
            _maxArmor = maxArmor;
            _percentageOfArmorAbsorption = percentageOfArmorAbsorption;
            
            _currentHealth = _maxHealth;
            _currentArmor = _maxArmor;
        }
        

        public void DealDamage(float damage)
        {
            var damageReceived = damage;
            if (!IsArmorDestroyed)
            {
                var armorNeeded = damage * _percentageOfArmorAbsorption / 100;
                if (_currentArmor - armorNeeded < _minArmor)
                {
                    damageReceived -= _currentArmor;
                    _currentArmor = _minArmor;
                }
                else
                {
                    damageReceived -= armorNeeded;
                    _currentArmor -= armorNeeded;
                }

                OnUpdateArmor?.Invoke(_currentArmor);
            }

            if (damageReceived < 0)
            {
                Debug.LogError("Damage received cannot be negative");
                return;
            }

            _currentHealth -= damageReceived;
            OnUpdateHealth?.Invoke(_currentHealth);
        }
    }
}