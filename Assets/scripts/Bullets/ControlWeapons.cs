using UnityEngine;
using static PlayerData;

public class ControlWeapons : MonoBehaviour
{
    public bool hasWeapon = false;

    [Header("Weapon Config")]
    public GameObject pickedWeapon;
    public BulletCollection bulletCollection;


    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("wapon") && !hasWeapon)
        {
            pickedWeapon = other.gameObject; 
            hasWeapon = true;
            other.GetComponent<SpriteRenderer>().enabled = false; 
            other.GetComponent<BoxCollider>().enabled = false; 
            bulletCollection.prefabs.Add(pickedWeapon);


            BulletStyle newStyle = new BulletStyle();
            newStyle.Initiazlize(BulletType.SNIPER, pickedWeapon);
            bulletCollection.bulletCollection.Add(newStyle);

            PlayerBase player = FindAnyObjectByType<PlayerBase>();

            PlayerBase.Action newAction = new PlayerBase.Action(
                PlayerBase.ActionType.ACTIVE,
                PlayerBase.ActionEnum.ONESHOT,            
                (KeyCode)System.Enum.Parse(typeof(KeyCode), "Alpha" + (player.playerData.availableActions.Count + 1)), 
                cost: 3,
                style: newStyle
            );

            player.AddNewAction(newAction);
            hasWeapon = false;
        }
    }

    void Update()
    {
       
    }
}