using Planets.PlayerPlanet;
using Players;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Dialog.InfoAboutPlayerPlanet
{
    public class InfoAboutPlayerPlanetDialog : DialogBase
    {
        [SerializeField] private Text _playerName;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private CreatorShip _creatorShip;
        [SerializeField] private PlayerGoldQuantity _goldQuantity;
        [SerializeField] private CreatingShipTime _creatingShipTime;

        private IPlayerPlanet _planet;
        private Transform _planetTransform;

        private bool _isInitialized;

        private void OnEnable()
        {
            Main.Instance.OnUpdateGame += CustomUpdate;
        }

        public void Init(IPlayerPlanet planet, PlayerType player, Transform planetTransform)
        {
            if (_isInitialized)
            {
                return;
            }
            
            _planet = planet;
            _playerName.text = PlayerUtils.GetPlayerName(player);
            _planetTransform = planetTransform;
            _isInitialized = true;

            _creatorShip.Init(planet.CreateRandomShipDebug);
            _goldQuantity.SetGoldValue(_planet.CurrentGold);
            _creatingShipTime.SetColorSlider(Main.Instance.ColorStorage.GetColorForPlayer(player));
            _rect.PinUIObjectToObjectOnScene(_planetTransform.position);
        }

        public void UpdateCurrentGold()
        {
            _goldQuantity.SetGoldValue(_planet.CurrentGold);
        }
        
        private void CustomUpdate()
        {
            if (_planet.Production == null)
            {
                _creatingShipTime.ResetAll();
                return;
            }

            var nameShip = PlayerUtils.GetShipName(_planet.Production.ShipType);
            var currentTime = _planet.Production.CurrentTime;
            var maxTime = _planet.Production.MaxTime;
            _creatingShipTime.SetSlider(currentTime, 0, maxTime);
            _creatingShipTime.SetShipName(nameShip);
        }

        public override void Hide()
        {
            base.Hide();
            _isInitialized = false;
        }

        private void OnDisable()
        {
            Main.Instance.OnUpdateGame -= CustomUpdate;
        }
    }
}