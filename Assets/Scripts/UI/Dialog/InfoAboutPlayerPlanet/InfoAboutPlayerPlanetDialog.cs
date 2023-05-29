using System;
using Planets.PlayerPlanet;
using UnityEngine;
using Utils;

namespace UI.Dialog.InfoAboutPlayerPlanet
{
    public class InfoAboutPlayerPlanetDialog : DialogBase, IDisposable
    {
        [SerializeField] private RectTransform _rect;
        [SerializeField] private CreatorShip _creatorShip;
        [SerializeField] private PlayerGoldQuantity _goldQuantity;

        private IPlayerPlanet _planet;
        private Transform _planetTransform;

        private bool _isInitialized;
        
        public void Init(IPlayerPlanet planet, Transform planetTransform)
        {
            if (_isInitialized)
            {
                return;
            }
            
            _planet = planet;
            _planetTransform = planetTransform;
            Main.Instance.OnUpdateGame += CustomUpdate;
            _isInitialized = true;

            _creatorShip.Init(planet.CreateRandomShipDebug);
            _goldQuantity.SetGoldValue(_planet.CurrentGold);
            _rect.PinUIObjectToObjectOnScene(_planetTransform.position);
        }

        public void UpdateCurrentGold()
        {
            _goldQuantity.SetGoldValue(_planet.CurrentGold);
        }
        
        private void CustomUpdate()
        {
            
        }

        public override void Hide()
        {
            base.Hide();
            Dispose();
            _isInitialized = false;
        }

        public void Dispose()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            Main.Instance.OnUpdateGame -= CustomUpdate;
        }
    }
}