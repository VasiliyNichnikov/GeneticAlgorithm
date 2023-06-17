using System.Collections.Generic;
using System.Linq;
using Planets;

namespace ShipLogic
{
    public class WeightCalculator
    {
        private readonly IShipDetector _detector;

        public WeightCalculator(IShipDetector detector)
        {
            _detector = detector;
        }
        
        public float GetWeight()
        {
            var alliesWeight = GetSumWeights(_detector.Allies);
            var enemyWeight = GetSumWeights(_detector.Enemies);
            var planetWeight = GetSumWeights(_detector.Planets);

            if (alliesWeight > enemyWeight)
            {
                return planetWeight;
            }

            return enemyWeight - alliesWeight + planetWeight;
        }

        private static float GetSumWeights(IEnumerable<ITarget> targets)
        {
            return targets.Sum(enemy => enemy.ThreatLevel);
        }
    }
}