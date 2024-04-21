using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Archer : Unit
{

    GameObject archerModel;
    
    public void changeFinalPosition(float x, float y, float z)
    {
        finalPosition = new Vector3(x, y, z);
    }

    protected override void attackTo()
    {
        throw new System.NotImplementedException();
    }

    protected override void hold()
    {
        throw new System.NotImplementedException();
    }

    protected override void spawn()
    {
        spawnPosition = new Vector3(0, 3, 0);
        archerModel = Instantiate(unitPrefab, spawnPosition, Quaternion.identity);
    }

    protected override void moveTo()
    {
        if (transform.position == finalPosition || finalPosition == null)
        {
            return;
        } else {
            transform.position = Vector3.MoveTowards(transform.position, finalPosition, 0.1f);
        }
    }

}
