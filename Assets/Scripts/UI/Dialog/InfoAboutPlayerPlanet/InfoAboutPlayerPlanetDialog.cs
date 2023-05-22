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
        }
        
        private void CustomUpdate()
        {
            _rect.PinUIObjectToObjectOnScene(_planetTransform.position);
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