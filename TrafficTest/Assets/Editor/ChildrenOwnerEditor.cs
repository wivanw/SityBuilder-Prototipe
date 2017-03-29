using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ChildrenOwner))]
public class ChildrenOwnerEditor : Editor
{
    private ChildrenOwner _mObject;
    private SerializedObject _msObject;

    private void OnEnable()
    {
        _mObject = target as ChildrenOwner;
        _msObject = new SerializedObject(_mObject);
    }

    public override void OnInspectorGUI()
    {
        _mObject.LocalPosition = new Hex(Mathf.RoundToInt(_mObject.transform.position.x), Mathf.RoundToInt(_mObject.transform.position.z));
        if (GUILayout.Button("Выровнять"))
        {
            _mObject.transform.localPosition = new Vector3(Mathf.RoundToInt(_mObject.transform.localPosition.x),
                _mObject.transform.localPosition.y, Mathf.RoundToInt(_mObject.transform.localPosition.z));
        }
        _msObject.ApplyModifiedProperties();
        DrawDefaultInspector();
    }
}
