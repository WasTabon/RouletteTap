using TMPro;
using UnityEngine;

public class NumberButton : MonoBehaviour
{
    [SerializeField] private int _id;
    [SerializeField] private TextMeshPro _text;
    
    [SerializeField] private Transform _center;

    private void Start()
    {
        _text.text = _id.ToString();
        LookAt(_center);
    }

    private void Update()
    {
        HandleTap();
    }

    private void CheckClick(Vector2 screenPosition)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPosition);
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
    
        if (hit.collider != null)
        {
            Debug.Log("Clicked on 2D object: " + hit.collider.name);
        }
    }

    private void LookAt(Transform cener)
    {
        Vector3 direction = cener.position - transform.position;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, 0, angle);
    }
    
    private void HandleTap()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckClick(Input.mousePosition);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            CheckClick(Input.GetTouch(0).position);
        }
    }
    
    private void OnValidate()
    {
        gameObject.name = $"NumberButton {_id}";
    }
}
