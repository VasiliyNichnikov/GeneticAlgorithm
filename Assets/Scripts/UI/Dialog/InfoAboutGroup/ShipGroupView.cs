using Group;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace UI.Dialog.InfoAboutGroup
{
    public class ShipGroupView : DialogBase
    {
        [SerializeField] private Text _title;
        [SerializeField] private Image _image;
        
        private RectTransform _rect;
        private IShipGroup _group;

        public void Init(IShipGroup group, string title, Color background)
        {
            _group = group;
            _rect = transform as RectTransform;
            _title.text = title;
            _image.color = background;

            Main.Instance.OnUpdateGame += CustomUpdate;
        }

        private void CustomUpdate()
        {
            // Желательно написать логику, которая будет только один раз выкл/вкл, а не каждый кадр
            gameObject.SetActive(_group.IsAlive);
            if (!_group.IsAlive)
            {
                return;
            }
            _rect.PinUIObjectToObjectOnScene(_group.CenterGroup);
        }

        private void OnDestroy()
        {
            Main.Instance.OnUpdateGame -= CustomUpdate;
        }
    }
}