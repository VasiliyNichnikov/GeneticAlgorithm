using Map;
using UnityEngine;

namespace UI.Dialog.GameController
{
    public class GameControllerDialog : DialogBase
    {
        [SerializeField, Header("SpeedGame")] private SpeedGame _speedGame;
        [SerializeField] private int _minSpeedGame;
        [SerializeField] private int _maxSpeedGame;

        [SerializeField] private GridVisibilitySettings _gridVisibility;

        private void Start()
        {
            _speedGame.Init(_minSpeedGame, _maxSpeedGame);
            _gridVisibility.Init(SpaceMap.Map.GridViewing);
        }
    }
}