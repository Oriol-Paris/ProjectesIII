using UnityEngine;

public class granada : MonoBehaviour
{
 
    private float targetingTime = 3f;
    private float heightArc = 5f;
    private float travelTime = 3f;

    [Header("Desviación")]
    public float deviationRange = 2f;

    [Header("Marcador visual")]
    public GameObject impactMarkerPrefab;
    public GameObject collisionImact;


    private  GameObject player;
    private Vector3 targetPosition;
    private GameObject markerInstance;
   
    private bool hasLaunched = false;

    void Start()
    {
        player = GameObject.Find("Player");
        if (player == null)
        {
          
        }

        // Calcular una vez la posición del objetivo con una pequeña desviación aleatoria
        Vector3 offset = new Vector3(
            Random.Range(-deviationRange, deviationRange),
            0,
            Random.Range(-deviationRange, deviationRange)
        );

        targetPosition = player.transform.position + offset;

        // Instanciar el marcador en el suelo
        markerInstance = Instantiate(impactMarkerPrefab, targetPosition, Quaternion.identity);

        // Empezar la cuenta atrás antes del lanzamiento
        Invoke(nameof(BeginLaunch), targetingTime);
    }



        private void BeginLaunch()
    {
        if (!hasLaunched)
        {
            hasLaunched = true;
            StartCoroutine(LaunchToTarget());
        }
    }

    private System.Collections.IEnumerator LaunchToTarget()
    {
        Vector3 startPos = transform.position;
        float elapsed = 0f;
      
        while (elapsed < travelTime)
        {
           if(player.GetComponent<TimeSecuence>().GetIsExecuting())
            {
                elapsed += Time.deltaTime;
                float t = elapsed / travelTime;

                // Interpolación horizontal
                Vector3 horizontalPos = Vector3.Lerp(startPos, targetPosition, t);

                // Altura parabólica
                float height = Mathf.Sin(t * Mathf.PI) * heightArc;
                transform.position = new Vector3(horizontalPos.x, horizontalPos.y + height, horizontalPos.z);
            }
            

            yield return null;
        }

        OnImpact();
    }

    private void OnImpact()
    {
        if (markerInstance != null)
            Destroy(markerInstance);
        // Llamada a tu función de daño en área
        SendMessage("ApplyAreaDamage", SendMessageOptions.DontRequireReceiver);

        collisionImact.SetActive(true);


        Destroy(gameObject, 1f);
    }
}