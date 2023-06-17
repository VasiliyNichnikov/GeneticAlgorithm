using System;
using System.Linq;
using Factories;
using Group;
using Loaders;
using Players;
using ShipLogic;
using Storages;
using UI.Dialog;
using UI.Dialog.GameController;
using UnityEngine;

public class Main : MonoBehaviour
{
    public event Action OnUpdateGame;

    public static Main Instance { get; private set; }

    public PlanetStorage PlanetStorage { get; private set; }
    public ShipFactory ShipFactory { get; private set; }
    public PlayerBrains Players { get; set; }
    public ShipGroupManager ShipGroupManager => _shipGroupManager;
    public DialogManager DialogManager => _dialogManager;
    public MainShipParameters ShipParameters => _shipParameters;
    public FactoryShip FactoryShip => _factoryShip;
    public Canvas LocationCanvas => _locationCanvas;
    public MaterialStorage MaterialStorage => _materialStorage;
    public ColorStorage ColorStorage => _colorStorage;
    public ILoaderManager LoaderManager => _loaderManager;
    public bool IsDebugMode => _isDebugMode;

    [SerializeField] private MainShipParameters _shipParameters;
    [SerializeField] private RectTransform _parentForDialogs;
    [SerializeField] private RectTransform _parentForLocationUI;
    [SerializeField] private Canvas _locationCanvas;
    [SerializeField] private FactoryShip _factoryShip;
    [SerializeField] private MaterialStorage _materialStorage;
    [SerializeField] private ColorStorage _colorStorage;
    [SerializeField] private bool _isDebugMode;
    
    private DialogManager _dialogManager;
    private GameControllerManager _gameManager;
    private LoaderManager _loaderManager;
    private ShipGroupManager _shipGroupManager;

    private void Awake()
    {
        Instance = this;

        ShipFactory = new ShipFactory();
        PlanetStorage = new PlanetStorage();
        _dialogManager = new DialogManager(_parentForDialogs, _parentForLocationUI);
        _gameManager = new GameControllerManager();
        Players = new PlayerBrains(PlanetStorage.GetMiningPlanets().ToArray());
        _loaderManager = new LoaderManager(this);

        _shipGroupManager = new ShipGroupManager();
        InitLoaders();
    }

    private void InitLoaders()
    {
        _loaderManager.AddLoaderInQueue(new MiningPlanetLoader());
        _loaderManager.AddLoaderInQueue(new SignalsLoader());
        _loaderManager.AddLoaderInQueue(new WeightsLoader());
    }

    private void Update()
    {
        OnUpdateGame?.Invoke();
    }

    private void OnDestroy()
    {
        ShipFactory.Dispose();
        _gameManager.Dispose();
        _shipGroupManager.Dispose();
    }
}