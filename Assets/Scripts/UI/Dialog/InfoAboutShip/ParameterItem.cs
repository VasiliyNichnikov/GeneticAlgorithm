using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class ParameterItem : MonoBehaviour
    {
        [SerializeField] private Text _nameParameter;
        [SerializeField] private Text _valueParameter;

        public void Init(string nameParameter, string valueParameter)
        {
            _nameParameter.text = nameParameter;
            _valueParameter.text = valueParameter;
        }
    }
}