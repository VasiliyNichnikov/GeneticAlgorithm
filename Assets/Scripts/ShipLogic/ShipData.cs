namespace ShipLogic
{
    public struct ShipData
    {
        public ShipSkinData.SkinType SkinType { get; private set; }
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
            ShipSkinData.SkinType skinType)
        {
            SpeedMovement = speedMovement;
            RateOfFire = rateOfFire;
            VisibilityRadius = visibilityRadius;
            GunPower = gunPower;
            Armor = armor;
            SkinType = skinType;
        }
    }
}