using UnityEngine;

[ExecuteAlways]
public class TriangleCircleVisualizer : MonoBehaviour
{
    public bool isVisualize;
    
    public int triangleCount = 12;
    public float radius = 5f;
    public float triangleBase = 1f; 
    public float triangleHeight = 2f; 

    private void OnDrawGizmos()
    {
        if (isVisualize)
        {
            Gizmos.color = Color.green;

            float angleStep = 360f / triangleCount;
            
            float triangleHeightAdjusted = Mathf.Sqrt(3f) / 2f; 

            for (int i = 0; i < triangleCount; i++)
            {
                float angleDeg = i * angleStep;
                float angleRad = angleDeg * Mathf.Deg2Rad;
                
                Vector3 center = transform.position;
                
                Vector3 baseCenter = center + new Vector3(Mathf.Cos(angleRad), Mathf.Sin(angleRad)) * radius;
                
                Quaternion rotation = Quaternion.Euler(0, 0, angleDeg - 90f); 
                
                Vector3 tip = center;
                Vector3 left = baseCenter + rotation * new Vector3(-triangleBase / 2f, 0f);
                Vector3 right = baseCenter + rotation * new Vector3(triangleBase / 2f, 0f);
                
                Gizmos.DrawLine(tip, left);
                Gizmos.DrawLine(tip, right);
                Gizmos.DrawLine(left, right);
                
                float baseWidth = 2f * radius * Mathf.Tan(Mathf.Deg2Rad * angleStep / 2f);
            }
        }
    }
}