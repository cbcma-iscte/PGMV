using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Unit
{

    GameObject mageModel;
    protected override void moveTo()
    {
        throw new System.NotImplementedException();
    }

    protected override void attackTo()
    {
        throw new System.NotImplementedException();
    }

    protected override void spawn()
    {
        spawnPosition = new Vector3(0, 3, 0);
        mageModel = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
    }

    protected override void hold()
    {
        throw new System.NotImplementedException();
    }
}
