using UnityEngine;

public class flash_gun : MonoBehaviour
{
   
    public float radius = 5f;
    public float flashIntensity = 10f;
    public float flashSpeed = 15f;
    public Light pointLight;

    

   

    void Start()
    {
      
        pointLight.intensity = 0f;
        StartCoroutine(FlashLight());
    }

   

    System.Collections.IEnumerator FlashLight()
    {
     

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
        Destroy(this.gameObject); ;
    }
}