using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleAnimation : MonoBehaviour
{
    [SerializeField] GameObject attacker;
    [SerializeField] GameObject defender;

    [SerializeField] Animator attackerAnimator;
    [SerializeField] Animator defenderAnimator;
    
    [SerializeField] float speedWalking;
    [SerializeField] float speedRunning;

    string[] allAnimationDefender = { "isDodging", "isBlocking", "isDying" };
    string[] allAnimationAttacker = { "isDodging", "isBlocking" };

    bool isBattleActive = true;
    float stateTimer;
    enum BattleState { Moving, AttackerTurn, DefenderTurn, Wait }
    BattleState currentState = BattleState.Moving;
    BattleState lastState;

    // Start is called before the first frame update
    void Start()
    {
        attackerAnimator = attacker.GetComponent<Animator>();
        defenderAnimator = defender.GetComponent<Animator>();
    }

    void DeactivateAllAnimations()
    {
        attackerAnimator.SetBool("isSlashing", false);
        attackerAnimator.SetBool("isBlocking", false);
        attackerAnimator.SetBool("isDodging", false);

        defenderAnimator.SetBool("isSlashing", false);
        defenderAnimator.SetBool("isBlocking", false);
        defenderAnimator.SetBool("isDodging", false);
    }

    void Update()
    {
        if (isBattleActive)
        {
            stateTimer -= Time.deltaTime;

            switch (currentState)
            {
                case BattleState.Moving:
                    HandleMovement();
                    break;
                case BattleState.AttackerTurn:
                    HandleAttackerTurn();
                    break;
                case BattleState.DefenderTurn:
                    HandleDefenderTurn();
                    break;
                case BattleState.Wait:
                    if (stateTimer <= 0f)
                    {

                        DeactivateAllAnimations();

                        attackerAnimator.SetBool("isIdle", true);
                        defenderAnimator.SetBool("isIdle", true);

                        if (lastState == BattleState.AttackerTurn)
                            currentState = BattleState.DefenderTurn;
                        else if (lastState == BattleState.DefenderTurn)
                            currentState = BattleState.AttackerTurn;
                    }
                    break;
            }
        }
    }

    void HandleMovement()
    {
        float distance = Vector3.Distance(attacker.transform.localPosition, defender.transform.localPosition);

        if (distance > 3f)
        {
            attacker.transform.localPosition = Vector3.MoveTowards(attacker.transform.localPosition, defender.transform.localPosition, speedRunning * Time.deltaTime);
            attackerAnimator.SetBool("isRunning", true);
            attackerAnimator.SetBool("isWalking", false);
        }
        else if (distance > 0.4f && distance <= 3f)
        {
            attacker.transform.localPosition = Vector3.MoveTowards(attacker.transform.localPosition, defender.transform.localPosition, speedWalking * Time.deltaTime);
            attackerAnimator.SetBool("isWalking", true);
            attackerAnimator.SetBool("isRunning", false);
        }
        else if (distance <= 0.4f)
        {
            attackerAnimator.SetBool("isWalking", false);
            attackerAnimator.SetBool("isRunning", false);
            attackerAnimator.SetBool("isIdle", true);
            currentState = BattleState.AttackerTurn;
        }
    }

    void HandleAttackerTurn()
    {
        // First time is slashing
        // Attacker attacks
        attackerAnimator.SetBool("isIdle", false);
        attackerAnimator.SetBool("isSlashing", true);

        // Defender Reacts
        string chosenAnimationDefender = AnimationDecidingDefender();
        defenderAnimator.SetBool(chosenAnimationDefender, true);
        defenderAnimator.SetBool("isIdle", false);

        if (chosenAnimationDefender == "isDying")
        {

            attackerAnimator.SetBool("isJumping",true);
            attackerAnimator.SetBool("isCelebrating",true);

            isBattleActive = false;
            // Leaves Scene
            return;
        }

        stateTimer = 2f;
        lastState = BattleState.AttackerTurn;
        currentState = BattleState.Wait;
    }

    void HandleDefenderTurn()
    {


        // Defender attacks
        string chosenAnimationDefender = "isSlashing";
        defenderAnimator.SetBool(chosenAnimationDefender, true);
        defenderAnimator.SetBool("isIdle", false);

        // Attacker reacts
        string chosenAnimationAttacker = AnimationDecidingAttackerResponse();
        attackerAnimator.SetBool(chosenAnimationAttacker, true);
        attackerAnimator.SetBool("isIdle", false);

        stateTimer = 2f;
        lastState = BattleState.DefenderTurn;
        currentState = BattleState.Wait;
    }

    string AnimationDecidingDefender()
    {
        int rand = Random.Range(0, allAnimationDefender.Length);
        return allAnimationDefender[rand];
    }

    string AnimationDecidingAttackerResponse()
    {
        int rand = Random.Range(0, allAnimationAttacker.Length);
        return allAnimationAttacker[rand];
    }
}
