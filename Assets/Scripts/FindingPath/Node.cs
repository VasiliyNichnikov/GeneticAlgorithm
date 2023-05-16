using JetBrains.Annotations;
using UnityEngine;

namespace FindingPath
{
    public class Node
    {
        public Vector3Int GridPoint { get; private set; }
        [CanBeNull] public Node Preview { get; private set; }

        public static Node Create(Vector3Int gridPosition)
        {
            return new Node() { GridPoint = gridPosition, Preview = null };
        }

        public void SetPreviewNode(Node preview)
        {
            Preview = preview;
        }

        private Node()
        {
        }

        public override bool Equals(object obj)
        {
            if (obj is Node node)
            {
                return GridPoint == node.GridPoint;
            }
        
            return false;
        }
    }
}