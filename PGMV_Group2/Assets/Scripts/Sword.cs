using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The Sword class handles the sword's attack behavior, including its movement towards a target and destruction after the attack.
/// </summary>
public class Sword : MonoBehaviour
{
    
    private bool isMEPressed = false;
    private bool isAttacking = false;
    private float speed = 1.5f;    
    Transform target;

    private void Awake(){
         target = new GameObject("target").transform;
    }
    
    /// <summary>
    /// Initiates the sword attack by setting the target's position and rotation.
    /// </summary>
    public void attack(){
        target.parent = transform.parent;
        target.transform.localPosition = new Vector3(-0.0469999984f,-0.178000003f,-0.115999997f);
        target.transform.rotation =  Quaternion.Euler(0.376240253f,238.920853f,194.933594f);
        isAttacking = true;
    }

    /// <summary>
    /// Coroutine to destroy the target and the sword game objects after a delay.
    /// </summary>
    private IEnumerator goDestroy(){
        yield return new WaitForSeconds(1f);
        Destroy(target.gameObject);
        Destroy(gameObject);
    }
    
    // Update is called once per frame
    void Update()
    {
        if(isAttacking && Vector3.Distance(transform.localPosition,target.localPosition)>0.05f){
            transform.rotation = Quaternion.Slerp(transform.rotation, target.rotation,  Time.deltaTime * speed);
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, target.localPosition, Time.deltaTime * 0.5f);
            
        }else if(isAttacking && Vector3.Distance(transform.localPosition,target.localPosition)<=0.05f){
            StartCoroutine(goDestroy());
            isAttacking=false;
        }
    }
}
