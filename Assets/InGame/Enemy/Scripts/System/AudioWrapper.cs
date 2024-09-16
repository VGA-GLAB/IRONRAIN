using UnityEngine;

namespace Enemy
{
    public static class AudioWrapper
    {
        /// <summary>
        /// SEを再生
        /// </summary>
        public static int PlaySE(string cueName)
        {
            return CriAudioManager.Instance.SE.Play("SE", cueName);
        }

        /// <summary>
        /// SEを再生(3D)
        /// </summary>
        public static int PlaySE(Vector3 position, string cueName)
        {
            return CriAudioManager.Instance.SE.Play3D(position, "SE", cueName);
        }

        /// <summary>
        /// SEを流す位置を更新
        /// </summary>
        public static void UpdateSePosition(Vector3 position, int index)
        {
            CriAudioManager.Instance.SE.Update3DPos(position, index);
        }

        /// <summary>
        /// SEを停止
        /// </summary>
        public static void StopSE(int index)
        {
            CriAudioManager.Instance.SE.Stop(index);
        }
    }
}