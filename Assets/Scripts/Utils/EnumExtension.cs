using System;

namespace Utils
{
    public static class EnumExtension
    {
        public static T ParseEnum<T>(this string value)
        {
            return (T) Enum.Parse(typeof(T), value, true);
        }
    }
}