using UnityEngine;

namespace Enemy.Boss
{
    /// <summary>
    /// 必要な情報を黒板に書き込む。
    /// </summary>
    public class Perception
    {
        private Transform _transform;
        private BlackBoard _blackBoard;
        private Transform _player;
        private DebugPointP _pointP;
        private MeleeEquipment _meleeEquip;

        // 破棄した後に更新処理がされないようのフラグ。
        private bool _isDisposed;

        public Perception(RequiredRef requiredRef)
        {
            _transform = requiredRef.Transform;
            _blackBoard = requiredRef.BlackBoard;
            _player = requiredRef.Player;
            _pointP = requiredRef.PointP;
            _meleeEquip = requiredRef.MeleeEquip;
        }

        /// <summary>
        /// 初期化。
        /// エリアを作成し、他必要な値を黒板に書き込む。
        /// </summary>
        public void Init()
        {
            // エリアを作成、黒板に書き込む。
            _blackBoard.Area = AreaCalculator.CreateBossArea(_transform.position);
            _blackBoard.PlayerArea = AreaCalculator.CreatePlayerArea(_player);
        }

        /// <summary>
        /// 各値を更新
        /// </summary>
        public void Update()
        {
            if (_isDisposed) return;

            // エリアの位置を更新
            _blackBoard.PlayerArea.Point = AreaCalculator.AreaPoint(_player);
            _blackBoard.Area.Point = AreaCalculator.AreaPoint(_transform);

            // プレイヤーのエリアと接触していた場合、自身のエリアをめり込まない丁度の位置に戻す。
            if (_blackBoard.Area.Collision(_blackBoard.PlayerArea))
            {
                _blackBoard.Area.Point = _blackBoard.Area.TouchPoint(_blackBoard.PlayerArea);
            }

            // 自身から点Pへのベクトルを黒板に書き込む。
            Vector3 ppv = _pointP.transform.position - _transform.position;
            float ppMag = ppv.magnitude;
            _blackBoard.PointPDistance = ppMag;
            _blackBoard.PointPDirection = ppMag > 1E-05F ? ppv / ppMag : Vector3.zero;

            // 自身からプレイヤーへのベクトルを黒板に書き込む。
            Vector3 pv = _player.position - _transform.position;
            _blackBoard.PlayerDirection = pv.normalized;
            _blackBoard.PlayerSqrDistance = pv.sqrMagnitude;

            // プレイヤーが近接攻撃が届く範囲にいるか。
            _blackBoard.IsWithinMeleeRange = _meleeEquip.IsWithinRange(_player.position);
            
            // ボス戦開始からの経過時間を更新。
            if (_blackBoard.IsBossStarted) _blackBoard.ElapsedTime += Time.deltaTime;

            // 仕様が決まっていないのでとりあえずの処理として、ボス戦開始から1秒後に登場完了とする。
            if (_blackBoard.ElapsedTime > 1.0f) _blackBoard.IsAppearCompleted = true;
        }

        /// <summary>
        /// 後始末。
        /// このクラスが黒板に書き込んだ参照をnullにする。
        /// </summary>
        public void Dispose()
        {
            _blackBoard.Area = null;
            _blackBoard.PlayerArea = null;

            _isDisposed = true;
        }

        /// <summary>
        /// 描画。
        /// </summary>
        public void Draw()
        {
            _blackBoard.PlayerArea?.Draw();
            _blackBoard.Area?.Draw();
        }
    }
}
