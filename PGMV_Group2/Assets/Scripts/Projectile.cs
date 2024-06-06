using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{   
    private bool isAttacking = false;
    private float speed = 1.5f;
    public Vector3 finalPosition;

    public Vector3 myPosition;
    void Awake()
    {
        myPosition  = new Vector3(transform.localPosition.x, 0.25f, transform.localPosition.z);
        finalPosition = myPosition;
    }

    public void AttackPosition(Vector3 attackPos){
        finalPosition = attackPos;
        transform.localPosition = myPosition;
        transform.LookAt(finalPosition);
        isAttacking = true;
       
    }

    // Update is called once per frame
    void Update()
    {   
        if( Vector3.Distance(transform.localPosition , finalPosition)>0.1 && isAttacking){

            transform.localPosition = Vector3.MoveTowards(transform.localPosition ,finalPosition, speed * Time.deltaTime);    
            
        }

        if(isAttacking &&  Vector3.Distance(transform.localPosition , finalPosition)<=0.1){ 
            isAttacking=false;
            Destroy(gameObject);
        }
        
    }

}
