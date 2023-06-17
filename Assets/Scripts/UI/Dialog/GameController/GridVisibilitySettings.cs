using System;
using System.Collections.Generic;
using Map;
using UnityEngine;

namespace UI.Dialog.GameController
{
    public class GridVisibilitySettings : MonoBehaviour
    {
        [SerializeField] private GridVisibilityParameter _parameterPrefab;
        [SerializeField] private RectTransform _holder;

        private GridViewing _gridViewing;

        private readonly Stack<GridVisibilityParameter> _activeParameters = new ();

        public void Init(GridViewing gridViewing)
        {
            _gridViewing = gridViewing;

            CreateParameters();
        }

        private void CreateParameters()
        {
            CreateParameterForMovement();
            CreateParameterForSectorPlayer1();
            CreateParameterForSectorPlayer2();
        }

        private void CreateParameterForMovement()
        {
            var parameter = GetNewParameter();
            parameter.Init("Сетка для движения", state => ChangeStateParameter(parameter, _gridViewing.SelectGridToDisplayForMovement, state));
        }

        private void CreateParameterForSectorPlayer1()
        {
            var parameter = GetNewParameter();
            parameter.Init("Сетка весов (Player1)", state => ChangeStateParameter(parameter, _gridViewing.SelectedGridToDisplayForSectorPlayer1, state));
        }
        
        private void CreateParameterForSectorPlayer2()
        {
            var parameter = GetNewParameter();
            parameter.Init("Сетка весов (Player2)", state => ChangeStateParameter(parameter, _gridViewing.SelectedGridToDisplayForSectorPlayer2, state));
        }
        
        private void ChangeStateParameter(GridVisibilityParameter parameter, Action onChangeToggle, bool state)
        {
            DeactivateOtherParameters();
            _activeParameters.Push(parameter);
            if (state)
            {
                onChangeToggle?.Invoke();
            }
            _gridViewing.ChangeVisibilityHeatMapVisual(state);
        }

        private void DeactivateOtherParameters()
        {
            while (_activeParameters.Count > 0)
            {
                var parameter = _activeParameters.Pop();
                parameter.OffToggle();
            }
        }

        private GridVisibilityParameter GetNewParameter()
        {
            var parameter = Instantiate(_parameterPrefab, _holder, false);
            return parameter;
        }
    }
}