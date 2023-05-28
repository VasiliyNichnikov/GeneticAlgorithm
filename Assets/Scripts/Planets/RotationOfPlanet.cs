using System;
using UnityEngine;

namespace Planets
{
    public class RotationOfPlanet : MonoBehaviour
    {
        [SerializeField] private float _speedRotation;

        private void Update()
        {
            transform.Rotate(Vector3.up * Time.deltaTime * _speedRotation, Space.World);
        }
    }
}