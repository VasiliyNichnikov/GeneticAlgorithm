using System;

namespace Players
{
    public class PlayerGoldManager
    {
        public float CurrentGold => _currentGold;
        
        private float _currentGold;

        public PlayerGoldManager(float startGold = 0.0f)
        {
            _currentGold = startGold;
        }

        public void AddGold(float value)
        {
            _currentGold += value;
        }
    }
}