using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Catapult : Unit
{
    GameObject catapultModel;
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
        catapultModel = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
    }

    protected override void hold()
    {
        throw new System.NotImplementedException();
    }
}
