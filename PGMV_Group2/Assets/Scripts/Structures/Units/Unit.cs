using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public string Id { get; set; }
    public string Role { get; set; }
    public string Type { get; set; }
    public string Action { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public GameObject roleObject;
    public Vector3 finalPosition{get; set;}

    public Vector3 attackPosition{get; set;}

    public Vector3 spawnPosition{get; set;}

    protected abstract void moveTo();
    protected abstract void attackTo();
    protected abstract void spawn();
    protected abstract void hold();

    public override bool Equals(object obj)
    {
        if (obj == null || GetType() != obj.GetType())
        {
            return false;
        }
        return this.Id == ((Unit)obj).Id;
    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public void Update(){
        switch(Action){
            case "move":
                moveTo();
                break;
            case "attack":
                attackTo();
                break;
            case "spawn":
                spawn();
                break;
            case "hold":
                hold();
                break;
        }
    }
}
