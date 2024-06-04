using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    
    private bool isMEPressed = false;
    private bool isAttacking = false;
    private float speed = 0.5f;
    public Vector3 rotationAxis = Vector3.down; 
   
    //POS Vector3(-0.137999997,0.143999994,0.151999995)
    public void attack(){
        isAttacking = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.O)){
            transform.Rotate(rotationAxis, speed * Time.deltaTime);
            
        }
    }
}
