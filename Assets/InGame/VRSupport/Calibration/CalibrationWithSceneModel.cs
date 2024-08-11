using Cysharp.Threading.Tasks;
using Meta.XR.MRUtilityKit;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRSupport
{
    // 前提としてMetaQuest3に空間データを登録する必要がある。
    // スキャン後、「家具を追加」で任意のラベルを付けたAnchorを配置し、それを位置合わせの基準とする。
    public class CalibrationWithSceneModel : MonoBehaviour
    {
        // ラベルはデバイス側で定義されており、任意で追加できないのでこれで全部。
        public enum Label
        {
            COUCH, BED, SCREEN, TABLE, LAMP, STORAGE, PLANT, OTHER
        }

        // ラベルに対応したオブジェクトを位置合わせする。
        [System.Serializable]
        private class AlignmentTarget
        {
            [SerializeField] private Label _label;
            [SerializeField] private Transform _object;
            [SerializeField] private Vector3 _offset;

            public Label Label => _label;
            public Transform Object => _object;
            public Vector3 Offset => _offset;
        }

        [Header("位置合わせするオブジェクト")]
        [SerializeField] private AlignmentTarget _rightLever;
        [SerializeField] private AlignmentTarget _leftLever;

        /// <summary>
        /// デバイスから空間データを読み込み、キャリブレーションを行う。
        /// </summary>
        /// <returns>キャリブレーション成功:true、失敗:false</returns>
        public async UniTask<bool> CalibrationAsync()
        {
            // 部屋のロードに失敗した場合、それ以降の処理は中断。
            if (!await LoadRoomAsync()) return false;

            MRUKRoom room = FindAnyObjectByType<MRUKRoom>();

            // 配置
            return Calibration(room);
        }

        /// <summary>
        /// 既に空間データを読み込んでいる場合、キャリブレーションを行う。
        /// </summary>
        /// <returns>キャリブレーション成功:true、失敗:false</returns>
        public bool Calibration(MRUKRoom room)
        {
            // 配置
            return Placement(room, _rightLever) && Placement(room, _leftLever);
        }

        // デバイスから空間データを読み込んで成功or失敗を返す。
        private async UniTask<bool> LoadRoomAsync()
        {
            // デバイスから空間データを読み込む。
            MRUK.LoadDeviceResult result = await MRUK.Instance.LoadSceneFromDevice();

            // 結果をチェック。
            if (result == MRUK.LoadDeviceResult.NoScenePermission)
            {
                Debug.LogError("セットアップされた空間データを読み込もうとしたが、ユーザーがアクセスを許可していない。");
                return false;
            }
            else if (result == MRUK.LoadDeviceResult.NoRoomsFound)
            {
                Debug.LogError("セットアップされた空間データを読み込もうとしたが、ユーザーが空間データのセットアップを行っていない。");
                return false;
            }
            else
            {
                // 成功
                return true;
            }
        }

        // ラベルが一致したMRUKAnchorの位置を基準に配置。
        private bool Placement(MRUKRoom room, AlignmentTarget target)
        {
            Transform anchor = null;

            // MRUKRoomの子になっているMRUKAnchorを全探索し、ラベルが一致しているか調べる。
            foreach (Transform child in room.transform)
            {
                if (!child.TryGetComponent(out MRUKAnchor mruka)) continue;

                // 1つのAnchorに対して複数のラベルを登録できるので全て調べる。
                foreach (string label in mruka.AnchorLabels)
                {
                    // ラベルが一致していた場合はAnchorを確定。
                    if (label == target.Label.ToString()) { anchor = child; break; }
                }
            }

            // Anchorが見つからなかった場合。
            if (anchor == null) return false;

            if (target.Object != null)
            {
                // オフセット込みの位置に配置。
                target.Object.position = anchor.position + target.Offset;
            }

            return true;
        }
    }
}
