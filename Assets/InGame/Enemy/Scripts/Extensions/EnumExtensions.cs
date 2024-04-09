using System;

namespace Enemy.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// —ñ‹“Œ^‚Ì—v‘f”‚ğ•Ô‚·B
        /// </summary>
        public static int Length<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Length;
        }
    }
}
