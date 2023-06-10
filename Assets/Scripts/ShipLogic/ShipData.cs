namespace ShipLogic
{
    public struct ShipData
    {
        public ShipType ShipType { get; private set; }
        public float SpeedMovement { get; private set; }
        public float RateOfFire { get; private set; }
        public float VisibilityRadius { get; private set; }
        public float GunPower { get; private set; }
        public float Armor { get; private set; }
        
        public ShipData(float speedMovement, 
            float rateOfFire,
            float visibilityRadius, 
            float gunPower,
            float armor,
            ShipType shipType)
        {
            SpeedMovement = speedMovement;
            RateOfFire = rateOfFire;
            VisibilityRadius = visibilityRadius;
            GunPower = gunPower;
            Armor = armor;
            ShipType = shipType;
        }
    }
}