using UnityEngine;

public class flash_gun : MonoBehaviour
{
    public Transform player;
    public float radius = 5f;
    public float flashIntensity = 10f;
    public float flashSpeed = 15f;
    public Light pointLight;

    public float arcStartAngle = -90f; // ángulo mínimo (en grados)
    public float arcEndAngle = 90f;    // ángulo máximo (en grados)

    private Camera cam;
    private bool isFlashing = false;

    void Start()
    {
        cam = Camera.main;
        if (pointLight == null)
            pointLight = GetComponent<Light>();

        pointLight.intensity = 0f;
    }

    void Update()
    {
        if (player == null || cam == null) return;

        // Posición del mouse en el mundo
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = Mathf.Abs(cam.transform.position.z - player.position.z); // Para que funcione bien en 3D
        Vector3 worldMouse = cam.ScreenToWorldPoint(mousePos);

        // Vector del jugador al mouse (plano XZ)
        Vector3 flatDir = (worldMouse - player.position);
        flatDir.y = 0f; // Ignorar altura
        flatDir.Normalize();

        // Ángulo del mouse respecto al frente del jugador
        float angle = Mathf.Atan2(flatDir.x, flatDir.z) * Mathf.Rad2Deg;

        // Clamp al arco de media luna
        angle = Mathf.Clamp(angle, arcStartAngle, arcEndAngle);

        // Convertimos el ángulo en una dirección
        float angleRad = angle * Mathf.Deg2Rad;
        Vector3 dir = new Vector3(Mathf.Sin(angleRad), 0f, Mathf.Cos(angleRad));

        // Posición final con eje Y libre
        Vector3 newPos = player.position + dir * radius;
        newPos.y = transform.position.y; // mantiene Y actual (libre para modificar manualmente)
        transform.position = newPos;

        // Click = flash
        if (Input.GetMouseButtonDown(0) && !isFlashing)
        {
            StartCoroutine(FlashLight());
        }
    }

    System.Collections.IEnumerator FlashLight()
    {
        isFlashing = true;

        while (pointLight.intensity < flashIntensity)
        {
            pointLight.intensity += flashSpeed * Time.deltaTime;
            yield return null;
        }

        pointLight.intensity = flashIntensity;
        yield return new WaitForSeconds(0.05f);

        while (pointLight.intensity > 0f)
        {
            pointLight.intensity -= flashSpeed * Time.deltaTime;
            yield return null;
        }

        pointLight.intensity = 0f;
        isFlashing = false;
    }
}