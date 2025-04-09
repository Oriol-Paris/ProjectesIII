using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreViewGame : MonoBehaviour
{
    public List<GameObject> bullets = new List<GameObject>();
    private List<GameObject> instatiateBullets = new List<GameObject>();
    private GameObject[] bulletObjects;

    public GameObject ghostbullet;
    private Queue<GameObject> ghostBulletPool = new Queue<GameObject>(); 

    public bool can = false;

    void Start()
    {
       
        StartCoroutine(GhostExecute());
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Tab))
        {
            
            can = !can;

           
            if (can)
            {
                bulletObjects = GameObject.FindGameObjectsWithTag("Bullet");
                foreach (GameObject bullet in bulletObjects)
                {
                    if (!bullets.Contains(bullet))
                    {
                        bullets.Add(bullet); 
                    }
                }
            }
            else
            {
                foreach (GameObject instatiateBullet in instatiateBullets)
                {
                    Destroy(instatiateBullet);
                }
                    bullets.Clear();
            }
        }
    }

    
    IEnumerator GhostExecute()
    {
        while (true)
        {
            if (can)
            {
               

                foreach (GameObject bullet in bullets)
                {
                    GameObject ghostBulletInstance;

                   
                    if (ghostBulletPool.Count > 0)
                    {
                        ghostBulletInstance = ghostBulletPool.Dequeue();
                        ghostBulletInstance.SetActive(true);
                    }
                    else
                    {
                        
                        ghostBulletInstance = Instantiate(ghostbullet);
                    }

                    ghostBulletInstance.transform.position = bullet.transform.position;
                    ghostBulletInstance.transform.rotation = bullet.transform.rotation;

                    ghostBulletInstance.GetComponent<ghostBullet>().takedirection(bullet.GetComponent<DestroyBullet>().shootDirection, bullet.GetComponent<DestroyBullet>().time);

                    instatiateBullets.Add(ghostBulletInstance);

                }
               

            }
            else
            {
               
                foreach (GameObject instatiateBullet in instatiateBullets)
                {
                    if (instatiateBullet != null)
                    {
                        instatiateBullet.SetActive(false);
                        ghostBulletPool.Enqueue(instatiateBullet); 
                    }
                }
                instatiateBullets.Clear();
                bullets.Clear();
               
            }

            yield return new WaitForSeconds(1.0f);
        }
    }
}

