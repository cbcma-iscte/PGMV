using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Role : MonoBehaviour
{
    public GameObject roleObject;
    public Vector3 finalPosition{get; set;}

    public Vector3 attackPosition{get; set;}

    public Vector3 spawnPosition{get; set;}

    protected abstract void moveTo();
    protected abstract void attackTo();
    protected abstract void spawn();
    protected abstract void hold();
    
    public void Start(){
        spawn();
    }
    public void Update(){
        moveTo();
        //attackTo();
        //hold();
    }
}
