using UnityEngine;
using UnityEditor;

public class ArrangeChildren : EditorWindow
{
    private Transform parentObject;
    private float xSpacing = 8f;

    [MenuItem("Tools/Arrange Children By X Spacing")]
    public static void ShowWindow()
    {
        GetWindow<ArrangeChildren>("Arrange Children");
    }

    void OnGUI()
    {
        GUILayout.Label("Arrange Children Settings", EditorStyles.boldLabel);

        parentObject = (Transform)EditorGUILayout.ObjectField("Parent Object", parentObject, typeof(Transform), true);
        xSpacing = EditorGUILayout.FloatField("X Spacing", xSpacing);

        if (GUILayout.Button("Arrange Now"))
        {
            if (parentObject != null)
            {
                Arrange();
            }
            else
            {
                Debug.LogWarning("Please assign a Parent Object!");
            }
        }
    }

    void Arrange()
    {
        if (parentObject == null) return;

        Vector3 startPos = parentObject.GetChild(0).position; // take first child’s current position
        for (int i = 0; i < parentObject.childCount; i++)
        {
            Transform child = parentObject.GetChild(i);
            Vector3 newPos = new Vector3(
                startPos.x + (xSpacing * i),
                startPos.y,
                startPos.z
            );
            Undo.RecordObject(child, "Arrange Children");
            child.position = newPos;
        }

        Debug.Log("Children arranged with spacing " + xSpacing + " on X axis.");
    }
}
