using UnityEngine;

[ExecuteAlways]
public class AudioDebugger : MonoBehaviour
{
    public AudioSource audioSource;
    public int spectrumSize = 64;
    public float heightMultiplier = 50f;
    public float barSpacing = 0.2f;
    public Color barColor = Color.cyan;
    public Color labelColor = Color.white;
    public Font labelFont;

    private float[] spectrum;

    void Update()
    {
        if (audioSource == null) return;
        if (spectrum == null || spectrum.Length != spectrumSize)
            spectrum = new float[spectrumSize];

        audioSource.GetSpectrumData(spectrum, 0, FFTWindow.BlackmanHarris);
    }

    void OnDrawGizmos()
    {
        if (spectrum == null || spectrum.Length == 0) return;

        Gizmos.color = barColor;
        Vector3 basePos = transform.position;

        for (int i = 0; i < spectrum.Length; i++)
        {
            float value = spectrum[i] * heightMultiplier;
            Vector3 pos = basePos + new Vector3(i * barSpacing, 0, 0);
            Vector3 top = pos + new Vector3(0, value, 0);

            Gizmos.DrawLine(pos, top);

#if UNITY_EDITOR
            UnityEditor.Handles.color = labelColor;
            UnityEditor.Handles.Label(top + Vector3.up * 0.2f, value.ToString("F2"));
#endif
        }
        DrawZone(0, 3, Color.red, "Bass");
        DrawZone(4, 10, Color.yellow, "Low-Mid");
        DrawZone(11, 30, Color.green, "Mid");
        DrawZone(31, 50, Color.blue, "High");
        DrawZone(51, 63, Color.magenta, "Ultra-High");
    }

    void DrawZone(int from, int to, Color color, string label)
    {
        Vector3 basePos = transform.position;
        float xStart = from * barSpacing;
        float xEnd = to * barSpacing;

        Vector3 corner1 = basePos + new Vector3(xStart, 0, 0);
        Vector3 corner2 = basePos + new Vector3(xEnd, heightMultiplier, 0);

#if UNITY_EDITOR
        UnityEditor.Handles.color = new Color(color.r, color.g, color.b, 0.1f);
        UnityEditor.Handles.DrawSolidRectangleWithOutline(new Vector3[]
        {
            corner1,
            new Vector3(xEnd, 0, 0) + basePos,
            corner2,
            new Vector3(xStart, heightMultiplier, 0) + basePos
        }, new Color(color.r, color.g, color.b, 0.1f), color);

        UnityEditor.Handles.Label(corner2 + Vector3.up * 1f, label, new GUIStyle()
        {
            normal = new GUIStyleState() { textColor = color },
            fontSize = 12,
            alignment = TextAnchor.MiddleCenter
        });
#endif
    }
}
