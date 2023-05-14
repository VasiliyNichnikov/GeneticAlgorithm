using System;
using Factory;
using UnityEngine;

public class Main : MonoBehaviour
{
    public event Action OnUpdateGame;
    
    public static Main Instance { get; private set; }

    public ShipProjectileFactory ProjectileFactory => _projectileFactory;
    
    [SerializeField] private ShipProjectileFactory _projectileFactory;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        OnUpdateGame?.Invoke();
    }
}