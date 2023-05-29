using System.Collections.Generic;
using System.Globalization;
using HandlerClicks;
using UI.Dialog.InfoAboutShip;

namespace ShipLogic
{
    public struct ParameterShipData
    {
        public string Name { get; set; }
        public string Value { get; set; } 
    }
    
    public class ShipClickHandler : IObjectToClick
    {
        private readonly ShipBase _ship;
        private readonly ShipData _data;

        public ShipClickHandler(ShipBase ship, ShipData calculatedShipData)
        {
            _ship = ship;
            _data = calculatedShipData;
        }
        
        public void Clicked()
        {
            var infoAboutShip = Main.Instance.DialogManager.GetDialog<InfoAboutShipDialog>();

            var parameters = new List<ParameterShipData>();
            parameters.Add(new ParameterShipData
            {
                Name = "Скорость корабля",
                Value = _data.SpeedMovement.ToString(CultureInfo.InvariantCulture)
            });
            parameters.Add(new ParameterShipData
            {
                Name = "Зона видимости",
                Value = _data.VisibilityRadius.ToString(CultureInfo.InvariantCulture)
            });
            parameters.Add(new ParameterShipData
            {
                Name = "Броня корабля",
                Value = _ship.Health.ArmorStats
            });
            parameters.Add(new ParameterShipData
            {
                Name = "Здоровье корабля",
                Value = _ship.Health.HealthStats
            });
            parameters.Add(new ParameterShipData
            {
                Name = "Мощность снаряда",
                Value = _data.GunPower.ToString(CultureInfo.InvariantCulture)
            });
            parameters.Add(new ParameterShipData
            {
                Name = "Скорость перезарядки",
                Value = _data.RateOfFire.ToString(CultureInfo.InvariantCulture)
            });
            parameters.Add(new ParameterShipData
            {
                Name = "Состояние корабля",
                Value = _ship.NameCurrentState
            });
            infoAboutShip.Init(parameters.AsReadOnly());
        }

        public void OnStartDrag()
        {
            // nothing
        }

        public void OnEndDrag()
        {
            // nothing
        }
    }
}