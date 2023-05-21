using System;
using System.Linq;
using HandlerClicks;
using Players;
using SpaceObjects;
using UnityEngine;

namespace ShipLogic
{
    public abstract class ShipBase : MonoBehaviour, IObjectToClick, ITargetToAttack, IDisposable
    {
        public DetectedObjectType ObjectType => DetectedObjectType.Ship;
        public PlayerType PlayerType => _playerType;
        public Vector3 Position => transform.position;
        public float ShipRadius => _shipData.RadiusShip;
        protected float SpeedMovement { get; private set; }

        public bool IsDead => Health.IsDead;

        public IShipHealth Health { get; private set; }
        public IShipEngine Engine { get; private set; }
        public abstract IShipGun Gun { get; protected set; }

        [SerializeField] protected ShipDetector Detector;
        
        // todo будет задаваться при создание корабля
        [SerializeField] private PlayerType _playerType;
        
        
        [SerializeField] private ShipSkinData[] _skins;
        
        public ShipData CalculatedShipData => _calculatedShipData;
        public string NameCurrentState => _commander.NameCurrentState;

        private ShipSkinData _shipData;
        private IShipCommander _commander;
        private bool _isInitialized;
        private ShipData _calculatedShipData;
        private ShipClickHandler _clickHandler;

        /// <summary>
        /// Процент поглащения урона броней
        /// </summary>
        private const float PercentageOfArmorAbsorption = 45f;


        private void Awake()
        {
            foreach (var skin in _skins)
            {
                if (skin == null)
                {
                    continue;
                }

                skin.gameObject.SetActive(false);
            }
        }


        public virtual void Init(IShipCommander commander, ShipData data)
        {
            InitSkin(data);
            _commander = commander;
            Detector.Init(_playerType, commander);
            _isInitialized = true;
        }

        public abstract bool SeeOtherShip(ITargetToAttack ship);

        public abstract bool CanAttackOtherShip(ITargetToAttack ship);
        
        public void TurnOffEngine()
        {
            if (!_isInitialized)
            {
                return;
            }

            _shipData.EffectsManager.TurnOffEngine();
        }

        public void TurnOnEngine()
        {
            if (!_isInitialized)
            {
                return;
            }

            _shipData.EffectsManager.TurnOnEngine();
        }

        public void DestroyShip()
        {
            if (!_isInitialized)
            {
                return;
            }

            _shipData.EffectsManager.DestroyShip();
        }
        
        public void OnDestroy()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_isInitialized)
            {
                return;
            }
        }

        private void InitSkin(ShipData data)
        {
            var shipData = _skins.FirstOrDefault(d => d.Skin == data.SkinType);
            if (shipData == null)
            {
                Debug.LogError($"Not found skin with type: {data.SkinType}");
                return;
            }

            _calculatedShipData = data;
            _shipData = shipData;
            name = $"{name}_{_shipData.Skin.ToString()}";

            shipData.gameObject.SetActive(true);
            Detector.SetRadius(data.VisibilityRadius);
            SpeedMovement = data.SpeedMovement;

            // Инициализация обработчика нажатий
            _clickHandler = new ShipClickHandler(this, _calculatedShipData);
            
            // Инициализация здоровья
            var minMaxHealth = Main.Instance.ShipParameters.GetParameterByType(MainShipParameters.ParameterType.Health);
            Health = new ShipHealth(minMaxHealth.minValue, minMaxHealth.maxValue, 0, data.Armor, PercentageOfArmorAbsorption);
            // Инициализация двигателя
            Engine = new ShipEngine(transform, _shipData.Agent, _shipData.RadiusShip, _calculatedShipData.SpeedMovement,
                _calculatedShipData.SpeedMovement);
            // Инициализация оружия
            Gun = new ShipGun(this, _shipData.GunPoints, _calculatedShipData.RateOfFire, _calculatedShipData.GunPower);
        }

        public void Clicked()
        {
            _clickHandler.Clicked();
        }


#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (_shipData == null || !_isInitialized)
            {
                return;
            }
            
            Gizmos.color = new Color(1, 0, 0, 0.25f);
            Gizmos.DrawSphere(transform.position, _shipData.RadiusShip);

            Gizmos.color = Color.blue;
            if (Engine?.PathDebug == null || Engine.PathDebug.corners.Length <= 0)
            {
                return;
            }

            var previewPoint = Engine.PathDebug.corners[0];
            for (var i = 0; i < Engine.PathDebug.corners.Length - 1; i++)
            {
                Gizmos.DrawLine(previewPoint, Engine.PathDebug.corners[i + 1]);
                previewPoint = Engine.PathDebug.corners[i + 1];
            }
        }
#endif
    }
}