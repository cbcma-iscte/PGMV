using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Role
{
    public void Awake()
    {
        finalPosition = transform.position;
    }
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
        throw new System.NotImplementedException();
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
