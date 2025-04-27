using UnityEngine;

public class ControlWapons : MonoBehaviour
{
    public bool hasSniper = false;

    [Header("wapon Config")]
    public GameObject objetoObtenido;
    public BulletCollection bulletCollection;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wapon") && !hasSniper)
        {
            objetoObtenido = other.gameObject; 
            hasSniper = true;
            objetoObtenido.SetActive(false);
            bulletCollection.prefabs.Add(objetoObtenido);

        }
    }

    void Update()
    {
        if (hasSniper && Input.GetKeyDown(KeyCode.Alpha6))
        {
           
            
        }
    }
}