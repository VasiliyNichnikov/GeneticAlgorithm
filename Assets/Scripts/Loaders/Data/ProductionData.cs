using System.Collections.Generic;

namespace Loaders.Data
{
    public struct ShipProduction
    {
        public string ShipType { get; set; }
        public float Price { get; set; }
        public float Time { get; set; }
    }
    
    public struct ProductionData
    {
        public float StartingGold { get; set; }
        public List<ShipProduction> ShipProductions { get; set; }
    }
}