using System;
using Planets.MiningPlayer;
using Players;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Dialog.InfoAboutMiningPlanet
{
    public class InfoAboutMiningPlanetDialog : DialogBase, IDisposable
    {
        [SerializeField] private Text _title;
        [SerializeField] private ProductionPerMinute _productionPerMinute;
        [SerializeField] private CapturingPoint _capturingPoint;
        [SerializeField] private RectTransform _rect;

        private IMiningPlanet _planet;
        private Transform _planetTransform;
        private bool _isInitialized;

        public void Init(IMiningPlanet planet, Transform planetTransform)
        {
            if (_isInitialized)
            {
                return;
            }

            _title.text = planetTransform.name;
            _planet = planet;
            _planetTransform = planetTransform;
            
            _planet.OnUpdateRemainingTime += ChangeSliderValue;
            _planet.OnUpdatePlayerType += ChangePlayerName;
            Main.Instance.OnUpdateGame += CustomUpdate;
            _isInitialized = true;
        }

        private void ChangeSliderValue(float time)
        {
            if (time == 0)
            {
                _capturingPoint.ResetSlider();
                return;
            }
            
            _capturingPoint.SetSlider(time, _planet.CaptureTime);
        }

        private void ChangePlayerName(PlayerType playerType)
        {
            switch(playerType)
            {
                case PlayerType.None:
                    _capturingPoint.ResetName();
                    break;
                case PlayerType.Player1:
                    _capturingPoint.SetPlayerName("Player One");
                    break;
                case PlayerType.Player2:
                    _capturingPoint.SetPlayerName("Player Two");
                    break;
            }
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
            
            _planet.OnUpdateRemainingTime -= ChangeSliderValue;
            _planet.OnUpdatePlayerType -= ChangePlayerName;
            Main.Instance.OnUpdateGame -= CustomUpdate;
        }
    }
}