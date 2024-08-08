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
    }
}