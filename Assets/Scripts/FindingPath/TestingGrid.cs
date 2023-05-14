using System;
using UnityEngine;

namespace FindingPath
{
    public class TestingGrid : MonoBehaviour
    {
        public Grid Grid { get; private set; }

        [SerializeField] private Vector3 _originPosition;
        [SerializeField] private int _width;
        [SerializeField] private int _length;
        [SerializeField] private float _cellSize;
        [SerializeField] private Transform _parentText;

        private void Start()
        {
            Grid = new Grid(_width, _length, _cellSize, _originPosition, _parentText);
        }
    }
}