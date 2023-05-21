namespace Utils
{
    public static class Converter
    {
        public static float ConvertFromOneRangeToAnother(float minOld, 
            float maxOld, 
            float minNew, 
            float maxNew, 
            float valueOld)
        {
            var rangeOld = maxOld - minOld;
            var rangeNew = maxNew - minNew;

            if (rangeOld == 0)
            {
                return minNew;
            }
            
            var value = (((valueOld - minOld) * rangeNew) / rangeOld) + minNew;
            return value;
        }
    }
}