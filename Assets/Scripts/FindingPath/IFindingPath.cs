using UnityEngine;

namespace FindingPath
{
    public interface IFindingPath
    {
        Vector3[] TryToFindPath(Vector3 startPosition, Vector3 endPosition, out bool isFound);
    }
}