using UnityEditor;

[CustomEditor(typeof(RobinOrrery))]
public class RobinOrreryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        RobinOrrery orrery = (RobinOrrery)target;

        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();

        if (EditorGUI.EndChangeCheck())
        {
            orrery.GenerateOrrarySystem();
        }
    }
}
