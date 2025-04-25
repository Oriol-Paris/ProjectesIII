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
            ControlWapons controlWapons = player.GetComponent<ControlWapons>();
            PlayerBase playerBase = player.GetComponent<PlayerBase>();

            if (collection != null && collection.prefabs.Count > 0)
            {
               
                collection.prefabs.RemoveAt(collection.prefabs.Count - 1);
                controlWapons.objetoObtenido = null;
                controlWapons.hasSniper = false;

                playerBase.availableActions.RemoveAll(action =>
                   action.m_style != null &&
                   action.m_style.bulletType == BulletType.SNIPER
               );

                playerBase.DeleteAction();

            }
        }
    }
}