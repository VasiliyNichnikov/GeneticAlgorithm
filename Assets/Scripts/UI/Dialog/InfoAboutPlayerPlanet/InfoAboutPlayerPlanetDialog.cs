using System;
using Planets.PlayerPlanet;
using Players;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Dialog.InfoAboutPlayerPlanet
{
    public class InfoAboutPlayerPlanetDialog : DialogBase, IDisposable
    {
        [SerializeField] private Text _playerName;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private CreatorShip _creatorShip;
        [SerializeField] private PlayerGoldQuantity _goldQuantity;

        private IPlayerPlanet _planet;
        private Transform _planetTransform;

        private bool _isInitialized;
        
        public void Init(IPlayerPlanet planet, PlayerType player, Transform planetTransform)
        {
            if (_isInitialized)
            {
                return;
            }
            
            _planet = planet;
            _playerName.text = PlayerUtils.GetPlayerName(player);
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