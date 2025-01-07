using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class shootPlayer : MonoBehaviour
{


    private List<Vector3> listOfShoot = new List<Vector3> { };
    private List<GameObject> visualPlayerAfterShoot = new List<GameObject> { };

    public GameObject preShoot;
    public GameObject bullet;

  

    public void PreShoot(Vector3 lastPosition)
    {
        listOfShoot.Add(Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10)));

        GameObject instantiatedObject = Instantiate(preShoot, lastPosition, Quaternion.identity);
        visualPlayerAfterShoot.Add(instantiatedObject);

        Directorio.Apuntar(instantiatedObject, lastPosition, listOfShoot[listOfShoot.Count-1]);
    }


   

    public void Shoot(int countShoot)
    {
        Destroy(visualPlayerAfterShoot[0]);
        visualPlayerAfterShoot.RemoveAt(0);
        Directorio.Apuntar(gameObject,transform.position, listOfShoot[countShoot]);
        GameObject instantiatedObject = Instantiate(bullet, transform.position, Quaternion.identity);
        Directorio.Apuntar(instantiatedObject, transform.position, listOfShoot[countShoot]);


      
    }
}
