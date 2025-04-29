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
            other.GetComponent<MeshRenderer>().enabled = false; 
            other.GetComponent<BoxCollider>().enabled = false; 
            bulletCollection.prefabs.Add(objetoObtenido);


            BulletStyle newStyle = new BulletStyle();
            newStyle.Initiazlize(BulletType.SNIPER, objetoObtenido);
            bulletCollection.bulletCollection.Add(newStyle);

            PlayerBase player = FindAnyObjectByType<PlayerBase>();

            PlayerBase.Action newAction = new PlayerBase.Action(
                PlayerBase.ActionType.ACTIVE,
                PlayerBase.ActionEnum.ESPECIALSHOOT,
                KeyCode.Alpha5, 
                cost: 1,
                style: newStyle
            );

            player.AddNewAction(newAction);
           // player.playerData.availableActions.Add(newAction);

            //player.SaveCurrentState();
        }
    }

    void Update()
    {
       
    }
}