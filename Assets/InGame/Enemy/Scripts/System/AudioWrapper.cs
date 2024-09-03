using UnityEngine;

namespace Enemy
{
    public static class AudioWrapper
    {
        /// <summary>
        /// SEを再生
        /// </summary>
        public static void PlaySE(string cueName)
        {
            CriAudioManager.Instance.SE.Play("SE", cueName);
        }

        /// <summary>
        /// SEを再生(3D)
        /// </summary>
        public static void PlaySE(Vector3 position, string cueName)
        {
            CriAudioManager.Instance.SE.Play3D(position, "SE", cueName);
        }
    }
}