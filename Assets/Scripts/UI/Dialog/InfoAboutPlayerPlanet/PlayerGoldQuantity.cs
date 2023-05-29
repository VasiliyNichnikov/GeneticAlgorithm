using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Dialog.InfoAboutPlayerPlanet
{
    public class PlayerGoldQuantity : MonoBehaviour
    {
        [SerializeField] private Text _goldValue;

        public void SetGoldValue(float value)
        {
            _goldValue.text = value.ToString(CultureInfo.InvariantCulture);
        }
    }
}