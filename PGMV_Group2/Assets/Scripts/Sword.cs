using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : MonoBehaviour
{
    
    private bool isMEPressed = false;
    private bool isAttacking = false;
    private float speed = 0.5f;
    public Vector3 rotationAxis = Vector3.down; 
   

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
