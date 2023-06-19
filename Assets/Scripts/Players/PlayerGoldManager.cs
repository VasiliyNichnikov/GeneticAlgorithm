using Loaders;
using ShipLogic;
using UnityEngine;

namespace Players
{
    public class PlayerGoldManager
    {
        public float CurrentGold { get; private set; }

        public PlayerGoldManager(float startGold = 0.0f)
        {
            if (startGold == 0.0f)
            {
                Main.Instance.LoaderManager.LoadAsync<ProductionLoader>(loader =>
                {
                    CurrentGold = loader.StartingGold;
                }, false);
            }
            else
            {
                CurrentGold = startGold;
            }
        }

        public bool CanMakeTransaction(float price)
        {
            return CurrentGold - price >= 0;
        }
        
        public void AddGold(float value)
        {
            CurrentGold += value;
        }

        public static bool CanMakeTransaction(float currentGold, ShipType ship)
        {
            var loader = Main.Instance.LoaderManager.Get<ProductionLoader>();
            if (loader == null)
            {
                return false;
            }
            return currentGold - loader.GetPriceForShip(ship) >= 0;
        }
        
        public void RemoveGold(float value)
        {
            if (CurrentGold - value < 0)
            {
                Debug.LogError("You can't go into the negative after the purchase");
                return;
            }

            CurrentGold -= value;
        }
    }
}