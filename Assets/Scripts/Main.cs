﻿using System;
using Factories;
using ShipLogic;
using Storages;
using UI.Dialog;
using UI.Dialog.GameController;
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
    public MaterialStorage MaterialStorage => _materialStorage;
    public ColorStorage ColorStorage => _colorStorage;

    [SerializeField] private MainShipParameters _shipParameters;
    [SerializeField] private RectTransform _parentForDialogs;
    [SerializeField] private RectTransform _parentForLocationUI;
    [SerializeField] private Canvas _locationCanvas;
    [SerializeField] private FactoryShip _factoryShip;
    [SerializeField] private MaterialStorage _materialStorage;
    [SerializeField] private ColorStorage _colorStorage;

    private DialogManager _dialogManager;
    private GameControllerManager _gameManager;

    private void Awake()
    {
        Instance = this;

        ShipManager = new ShipManager();
        _dialogManager = new DialogManager(_parentForDialogs, _parentForLocationUI);
        _gameManager = new GameControllerManager();
    }

    private void Update()
    {
        OnUpdateGame?.Invoke();
    }

    private void OnDestroy()
    {
        ShipManager.Dispose();
        _gameManager.Dispose();
    }
}