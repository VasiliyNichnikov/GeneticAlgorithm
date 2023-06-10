using UnityEngine;
using Utils;

namespace Group
{
    public interface ISupportedGroup : IPosition
    {
        void InitGroup(IShipGroup group);
    }
}