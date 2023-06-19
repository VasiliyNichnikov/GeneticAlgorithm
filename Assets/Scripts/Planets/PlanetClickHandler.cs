using HandlerClicks;
using UnityEngine;

namespace Planets
{
    public class PlanetClickHandler : MonoBehaviour, IObjectToClick
    {
        public interface IPlanetClick
        {
            void OnStartDrag();
            void OnEndDrag();
        }

        private IPlanetClick _planet;
        
        public void Init(IPlanetClick planet)
        {
            _planet = planet;
        }
        
        public void Clicked(Vector3 position)
        {
            // nothing
        }

        public void OnStartDrag()
        {
            _planet?.OnStartDrag();
        }

        public void OnEndDrag()
        {
            _planet?.OnEndDrag();
        }
    }
}