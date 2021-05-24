using UnityEngine;

public static class Utils
{
    public static void DrawLine(Transform parent, Vector3 end, Color color)
    {
        var line = new GameObject();
        line.name = "LineRenderer";
        line.transform.parent = parent;

        var lr = line.AddComponent<LineRenderer>();
        lr.material = new Material(Shader.Find("Sprites/Default"));
        lr.startColor = color;
        lr.startWidth = 0.1f;
        lr.SetPosition(0, parent.position);
        lr.SetPosition(1, end);

        MonoBehaviour.Destroy(line, Time.deltaTime);
    }
}
