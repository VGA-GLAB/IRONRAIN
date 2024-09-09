using UnityEngine;
using System.Collections;

public class DisplayScript : MonoBehaviour
{
    // イニシャライゼ―ションに使用
    void Start()
    {
        Debug.Log("displays connected: " + Display.displays.Length);
        // Display.displays[0] は主要デフォルトディスプレイで、常に ON。
        // 追加ディスプレイが可能かを確認し、それぞれをアクティベートします。
        if (Display.displays.Length > 1)
            Display.displays[1].Activate();
        if (Display.displays.Length > 2)
            Display.displays[2].Activate();
    }
    // Update がフレームごとに1 回呼び出されます
    void Update()
    {

    }
}