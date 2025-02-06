using UnityEngine;
using UnityEngine.Serialization;

public class CheckPoint : MonoBehaviour
{
    [FormerlySerializedAs("_buildingTest")] [SerializeField] private Building _building;
    
    private bool _isChecked;

    private void Start()
    {
        _isChecked = false;
    }
    // コライダーにぶつかった時にビルのAnimationを実行する
    private void OnTriggerEnter(Collider other)
    {
        if(_isChecked)
            return;
        
        _building.StartBuildingAnimation();
        Debug.Log("チェックポイントを通過");
        _isChecked = true;
    }
}
