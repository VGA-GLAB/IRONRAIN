using UnityEngine;

/// <summary>
/// ボス戦のフィールドの中心。
/// </summary>
public class PointP : MonoBehaviour
{
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 1.0f);
    }
}