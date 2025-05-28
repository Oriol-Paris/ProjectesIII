using UnityEngine;
using static PlayerData;

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
            other.GetComponent<SpriteRenderer>().enabled = false; 
            other.GetComponent<BoxCollider>().enabled = false; 
            bulletCollection.prefabs.Add(objetoObtenido);


            BulletStyle newStyle = new BulletStyle();
            newStyle.Initiazlize(BulletType.SNIPER, objetoObtenido);
            bulletCollection.bulletCollection.Add(newStyle);

            PlayerBase player = FindAnyObjectByType<PlayerBase>();

            PlayerBase.Action newAction = new PlayerBase.Action(
                PlayerBase.ActionType.ACTIVE,
                PlayerBase.ActionEnum.ESPECIALSHOOT,            
                (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + (player.playerData.availableActions.Count + 1)), 
                cost: 3,
                style: newStyle
            );

            player.AddNewAction(newAction);
            hasSniper = false;
        }
    }

    void Update()
    {
       
    }
}