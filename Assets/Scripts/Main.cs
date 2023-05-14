using System;
using Factory;
using FindingPath;
using UnityEngine;

public class Main : MonoBehaviour
{
    public event Action OnUpdateGame;
    
    public static Main Instance { get; private set; }

    public TrafficMap Map => _map;
    public ShipProjectileFactory ProjectileFactory => _projectileFactory;
    
    [SerializeField] private ShipProjectileFactory _projectileFactory;
    [SerializeField] private TrafficMap _map;

    private void Awake()
    {
        Instance = this;
        _map.Init();
    }

    private void Update()
    {
        OnUpdateGame?.Invoke();
    }
}