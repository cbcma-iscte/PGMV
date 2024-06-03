using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Animation : MonoBehaviour
{

    [SerializeField] GameObject attacker;
    [SerializeField] GameObject defender;

    [SerializeField] Animator attackerAnimator;
    [SerializeField] Animator defenderAnimator;
    
    [SerializeField] float speedWalking;
    [SerializeField] float speedRunning;


    Vector3 attackerPosition;
    Vector3 defenderPosition;


    // Start is called before the first frame update
    void Start()
    {
        attackerAnimator = attacker.GetComponent<Animator>();
        defenderAnimator = defender.GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Vector3.Distance(attacker.transform.localPosition,defender.transform.localPosition) > 0.6f && Vector3.Distance(attacker.transform.localPosition,defender.transform.localPosition) < 3f) {
            attacker.transform.localPosition = Vector3.MoveTowards(attacker.transform.localPosition,defender.transform.localPosition, speedWalking * Time.deltaTime);
            attackerAnimator.SetBool("isWalking",true);
            attackerAnimator.SetBool("isRunning",false);
        } else if (Vector3.Distance(attacker.transform.localPosition,defender.transform.localPosition) > 3f ) {
            attacker.transform.localPosition = Vector3.MoveTowards(attacker.transform.localPosition,defender.transform.localPosition, speedRunning * Time.deltaTime);
            attackerAnimator.SetBool("isRunning",true); 
        } else if ( defenderAnimator.GetBool("isDying") == true) {
            attackerAnimator.SetBool("isSlashing",false);
            attackerAnimator.SetBool("isIdle",true); // celebrate here instead
        } else {
            attackerAnimator.SetBool("isSlashing",true);
            attackerAnimator.SetBool("isWalking",false);
            defenderAnimator.SetBool("isDying",true);
        } 
    }
}
