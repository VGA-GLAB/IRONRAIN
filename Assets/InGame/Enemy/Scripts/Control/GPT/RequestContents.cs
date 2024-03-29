namespace Enemy.Control.GPT
{
    /// <summary>
    /// AIにリクエストする内容
    /// </summary>
    public static class RequestContents
    {
        // 最小/最大
        const int Min = -10;
        const int Max = 10;

        /// <summary>
        /// 一度のリクエストでAIにレスポンスしてもらう質問数。
        /// </summary>
        public const int QuestionCount = 2;

        /// <summary>
        /// 振舞わせるロール
        /// </summary>
        public static string Content()
        {
            return
                "あなたはゲームの難易度を調整するです。" +
                "ゲームをプレイするのは、ある程度このゲームに理解がある人です。" +
                "ただし、ゲームを面白くするには難易度の緩急が必要です。" +
                "2つ質問するので、各回答を/区切りで回答してください。以下は解答例です。'''" +
                "3/-3";
        }

        /// <summary>
        /// 一定間隔でリクエストする内容
        /// </summary>
        public static string Text(int cumFire, int hitFire, int dead, int escape, float cumLt, float minLt, float maxLt)
        {
            return
                $"弾を{cumFire}回発射しました。{hitFire}回当たりました。'''" +
                $"{dead}体が倒され、{escape}体は倒されませんでした。'''" +
                $"生存時間の平均は{cumLt / (dead + escape)}秒です。'''" +
                $"最短{minLt}秒で倒され、最長{maxLt}秒間生き残った個体がいます。'''" +
                $"質問1'''" +
                $"弾の発射頻度を調整することが出来ます。" +
                $"変化量を{Min}から{Max}の数値で答えてください。" +
                $"そのままにしておく場合は0と答えてください。'''" +
                $"質問2'''" +
                $"体力の量を調整することが出来ます。" +
                $"変化量を{Min}から{Max}の数値で答えてください。" +
                $"そのままにしておく場合は0と答えてください。";
        }

        /// <summary>
        /// AIからのレスポンスをデータとして扱えるよう数値に変換し、処理を実行。
        /// </summary>
        public static bool Convert(string response, int[] array)
        {
            string[] split = response.Split("/");
            for (int i = 0; i < split.Length; i++)
            {
                // 質問のうちどれか1つでもパース出来なかった場合は、その時点でfalseを返す。
                if (int.TryParse(split[i], out int value))
                {
                    array[i] = value;
                }
                else return false;
            }

            return true;
        }

        /// <summary>
        /// 値をAIの評価値の範囲から、任意の範囲にリマップする。
        /// </summary>
        public static float Remap(float a, float b, float value)
        {
            return Unity.Mathematics.math.remap(Min, Max, a, b, value);
        }
    }
}
