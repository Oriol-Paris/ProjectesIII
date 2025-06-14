using UnityEngine;

public class OneTimeShot : MonoBehaviour
{
    private void OnDestroy()
    {
        GameObject bulletPool = GameObject.Find("BulletPool");
        GameObject player = GameObject.Find("Player");
        if (bulletPool != null)
        {
            BulletCollection collection = bulletPool.GetComponent<BulletCollection>();
            ControlWeapons controlWapons = player.GetComponent<ControlWeapons>();
            PlayerBase playerBase = player.GetComponent<PlayerBase>();

            if (collection != null && collection.prefabs.Count > 0)
            {
               
                collection.prefabs.RemoveAt(collection.prefabs.Count - 1);
                controlWapons.pickedWeapon = null;
                controlWapons.hasWeapon = false;

                playerBase.availableActions.RemoveAll(action =>
                   action._style != null &&
                   action._style.bulletType == BulletType.SNIPER
               );
                
                
                KeyCode key = playerBase.activeAction._key;

                if (key >= KeyCode.Alpha1 && key <= KeyCode.Alpha9)
                {
                    int indexToRemove = (int)key - (int)KeyCode.Alpha1; // convierte la tecla en índice
                    if (indexToRemove >= 0 && indexToRemove < playerBase.availableActions.Count)
                    {
                        playerBase.DeleteAction(indexToRemove);
                        playerBase.ResetActiveAction();
                        FindAnyObjectByType<TopBarManager>().UpdateBottomHotbar();
                    }
                }
            }
        }
    }
}