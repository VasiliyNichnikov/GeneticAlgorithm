using System;
using ShipLogic;
using UI.Dialog;
using UnityEngine;

public class Main : MonoBehaviour
{
    public event Action OnUpdateGame;
    
    public static Main Instance { get; private set; }

    public DialogManager DialogManager => _dialogManager;
    public MainShipParameters ShipParameters => _shipParameters;
    
    [SerializeField] private MainShipParameters _shipParameters;
    [SerializeField] private RectTransform _canvasTransform;

    private DialogManager _dialogManager;
    
    private void Awake()
    {
        Instance = this;

        _dialogManager = new DialogManager(_canvasTransform);
    }

    private void Update()
    {
        OnUpdateGame?.Invoke();
    }
}