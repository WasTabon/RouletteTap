using UnityEngine;

public class ResponsiveCircleWorld : MonoBehaviour
{
    public float margin = 0.5f; // отступы в мировых координатах

    void Awake()
    {
        ResizeToScreen();
    }

    void ResizeToScreen()
    {
        Camera cam = Camera.main;

        // Левая и правая границы экрана в мировых координатах
        Vector3 left = cam.ViewportToWorldPoint(new Vector3(0, 0.5f, cam.nearClipPlane));
        Vector3 right = cam.ViewportToWorldPoint(new Vector3(1, 0.5f, cam.nearClipPlane));
        float worldWidth = (right - left).x - margin * 2;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        float spriteWidth = sr.sprite.bounds.size.x;

        float scale = worldWidth / spriteWidth;
        transform.localScale = new Vector3(scale, scale, 1);
    }
}
