using UnityEditor;
using UnityEngine;

/**
 * Title: 相机往指定方向平移的触发器、切换摄像机。
 * Description:
 */
public class CameraControlTrigger : MonoBehaviour
{
    public CameraControlObject cameraControl;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CameraManager._instance.CloseUp(cameraControl.closeUpOffset, cameraControl.closeUpduration, false);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CameraManager._instance.CloseUp(cameraControl.closeUpOffset, cameraControl.closeUpduration, true);
        }
    }
}

[System.Serializable]
public class CameraControlObject
{
    /// <summary>
    /// 特写
    /// </summary>
    public bool closeUp ;       
    [HideInInspector] public Vector3 closeUpOffset = new Vector3(0,2,0);
    [HideInInspector] public float closeUpduration = 0.3f;
};

[CustomEditor(typeof(CameraControlTrigger))]
public class CameraControlEditor : Editor
{
    CameraControlTrigger cameraControlTrigger;
    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        var obj = cameraControlTrigger.cameraControl;
        DrawDefaultInspector();
        if (obj.closeUp)
        {
            obj.closeUpOffset = EditorGUILayout.Vector3Field("closeUpOffset",obj.closeUpOffset);
            obj.closeUpduration = EditorGUILayout.FloatField("panDuration", obj.closeUpduration);
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}