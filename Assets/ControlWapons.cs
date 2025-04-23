using UnityEngine;

public class ControlWapons : MonoBehaviour
{
    private bool hasSniper = false;

    [Header("wapon Config")]
    public GameObject objetoObtenido;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wapon") && !hasSniper)
        {
            objetoObtenido = other.gameObject; 
            hasSniper = true;
            objetoObtenido.SetActive(false);
        }
    }

    void Update()
    {
        if (hasSniper && Input.GetKeyDown(KeyCode.Alpha6))
        {
           
            
        }
    }
}