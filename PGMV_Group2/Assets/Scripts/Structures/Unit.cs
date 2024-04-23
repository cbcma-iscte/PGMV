using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Unit : MonoBehaviour
{
    public string Id { get; set; }
    public string Role { get; set; }
    public string Type { get; set; }
    public string Action { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    private GameObject Prefab;
    public Vector3 finalPosition{get; set;}

    public Vector3 attackPosition{get; set;}

    public Vector3 spawnPosition{get; set;}

    public Unit(string id, string role, string type, string action, int x, int y)
    {
        Id = id;
        Role = role;
        Type = type;
        Action = action;
        X = x;
        Y = y;       
    }

     
    

    public void changeFinalPosition(float x, float y, float z)
    {
        finalPosition = new Vector3(x, y, z);
    }

    public void attackTo()
    {
        throw new System.NotImplementedException();
    }

    public void hold()
    {
        throw new System.NotImplementedException();
    }

    public GameObject spawn(GameObject prefab)
    {
        spawnPosition = new Vector3(0, 3, 0);
        Prefab = prefab;
        return Instantiate(prefab, spawnPosition, Quaternion.identity);
    }

    public void moveTo()
    {
        if (transform.position == finalPosition || finalPosition == null)
        {
            return;
        } else {
            transform.position = Vector3.MoveTowards(transform.position, finalPosition, 0.1f);
        }
    }
    
}
