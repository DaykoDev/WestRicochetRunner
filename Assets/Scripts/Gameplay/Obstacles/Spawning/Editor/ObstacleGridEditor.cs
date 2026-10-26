using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObstacleGrid))]
public sealed class ObstacleGridEditor : Editor
{
    private Vector2 _scrollPosition;

    public override void OnInspectorGUI()
    {
        ObstacleGrid grid = (ObstacleGrid)target;

        grid.EnsureInitialized();
        serializedObject.Update();

        SerializedProperty floor =  serializedObject.FindProperty("_floor");
        SerializedProperty localOffset = serializedObject.FindProperty("_localOffset");
        SerializedProperty cellSize = serializedObject.FindProperty("_cellSize");
        SerializedProperty allowedCells = serializedObject.FindProperty("_allowedCells");
        SerializedProperty showGrid = serializedObject.FindProperty("_showGrid");
        SerializedProperty allowedColor = serializedObject.FindProperty("_allowedColor");
        SerializedProperty blockedColor = serializedObject.FindProperty("_blockedColor");
        SerializedProperty lineColor = serializedObject.FindProperty("_lineColor");

        EditorGUILayout.PropertyField(floor);
        EditorGUILayout.PropertyField(localOffset);
        EditorGUILayout.PropertyField(cellSize);

        if (floor.objectReferenceValue == null)
        {
            EditorGUILayout.HelpBox(
            "Назначь объект пола с компонентом MeshFilter. " +
            "Размер и положение сетки будут рассчитаны по его Mesh.",
            MessageType.Info);
        }

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.Vector2IntField("Calculated Size", grid.Size);
        EditorGUILayout.IntField("Allowed Cells", grid.AllowedCellCount);
        EditorGUI.EndDisabledGroup();

        EditorGUILayout.Space();
        EditorGUILayout.PropertyField(showGrid);
        EditorGUILayout.PropertyField(allowedColor);
        EditorGUILayout.PropertyField(blockedColor);
        EditorGUILayout.PropertyField(lineColor);

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Allowed Cells", EditorStyles.boldLabel);

        if (grid.Size.x > 0 && grid.Size.y > 0)
        {
            DrawMaskButtons(allowedCells);
            DrawMask(grid.Size, allowedCells);
        }

        serializedObject.ApplyModifiedProperties();

        grid.EnsureInitialized();
    }

    private static void DrawMaskButtons(SerializedProperty allowedCells)
    {
        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Select all"))
        {
            SetAllCells(allowedCells, true);
        }

        if (GUILayout.Button("Clear"))
        {
            SetAllCells(allowedCells, false);
        }

        EditorGUILayout.EndHorizontal();
    }

    private static void SetAllCells(SerializedProperty allowedCells, bool value)
    {
        for (int i = 0; i < allowedCells.arraySize; i++)
        {
            allowedCells.GetArrayElementAtIndex(i).boolValue = value;
        }
    }

    private void DrawMask(Vector2Int size, SerializedProperty allowedCells)
    {
        _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition, GUILayout.MaxHeight(350f));

        for (int z = size.y - 1; z >= 0; z--)
        {
            EditorGUILayout.BeginHorizontal();

            for (int x = 0; x < size.x; x++)
            {
                int index = z * size.x + x;

                if (index >= allowedCells.arraySize)
                {
                    continue;
                }

                SerializedProperty cell = allowedCells.GetArrayElementAtIndex(index);

                cell.boolValue = GUILayout.Toggle(cell.boolValue, GUIContent.none, GUILayout.Width(20f));
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.EndScrollView();
    }
}