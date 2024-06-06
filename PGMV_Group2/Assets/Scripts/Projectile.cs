using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The class Projetile defines the behavior of a projectile in the game.
/// </summary>
public class Projectile : MonoBehaviour
{   
    private bool isAttacking = false;
    private float speed = 1.5f;
    public Vector3 finalPosition;

    public Vector3 myPosition;

    /// <summary>
    /// Initializes the initial and final positions of the projectile.
    /// </summary>
    void Awake()
    {
        myPosition  = new Vector3(transform.localPosition.x, 0.25f, transform.localPosition.z);
        finalPosition = myPosition;
    }

    /// <summary>
    /// Sets the target position for the projectile and initiates the attack animation.
    /// </summary>
    /// <param name="attackPos">The target position to attack.</param>
    public void AttackPosition(Vector3 attackPos){
        finalPosition = attackPos;
        transform.localPosition = myPosition;
        transform.LookAt(finalPosition);
        isAttacking = true;
       
    }

    /// <summary>
    /// Updates the position of the projectile every frame until it reaches the target position.
    /// After reaching the target position the projectile is destroyed.
    /// </summary>
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
