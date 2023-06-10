using JetBrains.Annotations;
using UnityEngine;

namespace Map
{
    public class Node
    {
        public Vector3Int GridPoint { get; }
        [CanBeNull] public Node Preview { get; private set; }

        public static Node Create(Vector3Int gridPosition)
        {
            return new Node(gridPosition) { Preview = null };
        }

        public void SetPreviewNode(Node preview)
        {
            Preview = preview;
        }

        private Node(Vector3Int gridPoint)
        {
            GridPoint = gridPoint;
        }

        public override bool Equals(object obj)
        {
            if (obj is Node node)
            {
                return GridPoint == node.GridPoint;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return GridPoint.GetHashCode();
        }
    }
}