using System;
using Factories;
using ShipLogic;
using UI.Dialog;
using UnityEngine;

public class Main : MonoBehaviour
{
    public event Action OnUpdateGame;
    
    public static Main Instance { get; private set; }

    public ShipManager ShipManager { get; private set; }
    public DialogManager DialogManager => _dialogManager;
    public MainShipParameters ShipParameters => _shipParameters;
    public FactoryShip FactoryShip => _factoryShip;
    public Canvas LocationCanvas => _locationCanvas;
    
    [SerializeField] private MainShipParameters _shipParameters;
    [SerializeField] private RectTransform _parentForDialogs;
    [SerializeField] private RectTransform _parentForLocationUI;
    [SerializeField] private Canvas _locationCanvas;
    [SerializeField] private FactoryShip _factoryShip;

    private DialogManager _dialogManager;

    private void Awake()
    {
        Instance = this;

        ShipManager = new ShipManager();
        _dialogManager = new DialogManager(_parentForDialogs, _parentForLocationUI);
    }

    private void Update()
    {
        OnUpdateGame?.Invoke();
    }
}