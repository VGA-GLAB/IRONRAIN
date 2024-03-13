using System;

namespace Enemy.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// �񋓌^�̗v�f����Ԃ��B
        /// </summary>
        public static int Length<T>() where T : Enum
        {
            return Enum.GetValues(typeof(T)).Length;
        }
    }
}
