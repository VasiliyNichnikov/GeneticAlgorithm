using System;
using System.Collections.Generic;
using Loaders;
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
        [SerializeField] private ProductionGold _productionGoldPrefab;
        [SerializeField] private CapturingPoint _capturingPoint;
        [SerializeField] private RectTransform _rect;
        [SerializeField] private RectTransform _holderForProduction;

        private readonly Dictionary<PlayerType, ProductionGold> _poolProductions = new Dictionary<PlayerType, ProductionGold>();

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
            
            _planet.OnUpdateRemainingTimeCatch += ChangeSliderValue;
            _planet.OnUpdatePlayerType += ChangePlayerNameCapturingPoint;
            _planet.OnUpdateRemainingTimeExtraction += CheckAndChangeProductionTime;
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
            
            _capturingPoint.SetSlider(1 - time, _planet.CaptureTime);
        }

        private void ChangePlayerNameCapturingPoint(PlayerType playerType)
        {
            var playerName = PlayerUtils.GetPlayerName(playerType);
            if (playerName == string.Empty)
            {
                _capturingPoint.ResetName();
                return;
            }
            _capturingPoint.SetPlayerName(playerName);
            _capturingPoint.SetColorSlider(Main.Instance.ColorStorage.GetColorForPlayer(playerType));
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
            
            _planet.OnUpdateRemainingTimeCatch -= ChangeSliderValue;
            _planet.OnUpdatePlayerType -= ChangePlayerNameCapturingPoint;
            _planet.OnUpdateRemainingTimeExtraction -= CheckAndChangeProductionTime;
            Main.Instance.OnUpdateGame -= CustomUpdate;
        }

        private void CheckAndChangeProductionTime(PlayerType player, float timeLeft)
        {
            var production = GetProduction(player);
            var loader = Main.Instance.LoaderManager.Get<MiningPlanetLoader>();

            if (loader == null)
            {
                return;
            }

            var minimumTime = loader.GetMinimumTimeMining();
            var maximumTime = loader.GetMaximumTimeMining();
            
            var currentValue = Converter.ConvertFromOneRangeToAnother(minimumTime,
                maximumTime, 0, 1, timeLeft);
            production.SetSlider(currentValue, minimumTime ,maximumTime);
        }
        
        private ProductionGold GetProduction(PlayerType player)
        {
            if (_poolProductions.TryGetValue(player, out var production))
            {
                return production;
            }

            var newProduction = Instantiate(_productionGoldPrefab, _holderForProduction, false);
            var color = Main.Instance.ColorStorage.GetColorForPlayer(player);
            var playerName = PlayerUtils.GetPlayerName(player);
            newProduction.Init(color, playerName);
            _poolProductions[player] = newProduction;
            return newProduction;
        }
    }
}