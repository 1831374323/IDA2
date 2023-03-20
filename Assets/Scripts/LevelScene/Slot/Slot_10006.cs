using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot_10006 : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EntitySpawner.Instance.EvolveCheck += Check;
    }

    private void FixedUpdate()
    {
        if (killedNum > 10)
        {
            //进化
        }
    }
    private void Check(Entity entity)
    {

        if (entity.EntityType == "dear")
        {
            entity.GetComponent<Entity_dear>().WhenKill += Kill;
        }
    }
    public int killedNum;
    void Kill()
    {
        killedNum++;
        Debug.Log("dear kill");
    }
}
