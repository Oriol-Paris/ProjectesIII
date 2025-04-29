using UnityEngine;

public class MouseIcon : MonoBehaviour
{
    public GameObject indicatorPrefab;
    public LayerMask groundLayer;
    private GameObject currentIndicator;
    public Vector3 mov;

    void Start()
    {
        if (indicatorPrefab != null)
        {
            currentIndicator = Instantiate(indicatorPrefab);
            
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, groundLayer))
        {
            if (currentIndicator != null)
            {
                currentIndicator.SetActive(true);
                currentIndicator.transform.position = (hit.point - mov) + Vector3.up * 0.01f; // un poco elevado para evitar z-fighting
                currentIndicator.transform.rotation = Quaternion.Euler(90f, 0f, 0f); // aplanado si es un sprite
            }
        }
    }

}
