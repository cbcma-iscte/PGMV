using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Rendering.LookDev;
using UnityEngine.SceneManagement;

public class BattleAnimation : MonoBehaviour
{

    [SerializeField] public CameraController look;
    [SerializeField] GameObject attacker;
    [SerializeField] GameObject defender;

    [SerializeField] Animator attackerAnimator;
    [SerializeField] Animator defenderAnimator;

    [SerializeField] float speedWalking;
    [SerializeField] float speedRunning;

    [SerializeField] Terrain terrain;
    [SerializeField] AudioSource slash;
    [SerializeField] AudioSource enemyDie;
    [SerializeField] AudioSource block;
    [SerializeField] AudioSource dodge;
    bool isGrounded = false;
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
        int heightMiddle = terrain.terrainData.heightmapResolution;
        attacker.transform.position = new Vector3(attacker.transform.position.x,terrain.terrainData.GetHeight(heightMiddle, heightMiddle) ,attacker.transform.position.z);
        defender.transform.position = new Vector3(defender.transform.position.x,terrain.terrainData.GetHeight(heightMiddle, heightMiddle) ,defender.transform.position.z);

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

    private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    void Update()
    {
        if (isBattleActive && isGrounded)
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

        attacker.transform.LookAt(defender.transform);

        if (distance > 20f)
        {
            attacker.transform.localPosition = Vector3.MoveTowards(attacker.transform.localPosition, defender.transform.localPosition, speedRunning * Time.deltaTime);

            attackerAnimator.SetBool("isRunning", true);
            attackerAnimator.SetBool("isWalking", false);
        }
        else if (distance > 8f && distance <= 20f)
        {
            attacker.transform.localPosition = Vector3.MoveTowards(attacker.transform.localPosition, defender.transform.localPosition, speedWalking * Time.deltaTime);
            attackerAnimator.SetBool("isWalking", true);
            attackerAnimator.SetBool("isRunning", false);
        }
        else if (distance <= 8f)
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
        CheckSound("isSlashing");

        // Defender Reacts
        string chosenAnimationDefender = AnimationDecidingDefender();
        defenderAnimator.SetBool(chosenAnimationDefender, true);
        defenderAnimator.SetBool("isIdle", false);
        CheckSound(chosenAnimationDefender);

        if (chosenAnimationDefender == "isDying")
        {
            CheckSound(chosenAnimationDefender);
            attackerAnimator.SetBool("isCelebrating",true);

            isBattleActive = false;
            // Leaves Scene
            StartCoroutine(LeaveScene());
        }

        stateTimer = 2f;
        lastState = BattleState.AttackerTurn;
        currentState = BattleState.Wait;
    }

    IEnumerator LeaveScene()
    {

        yield return new WaitForSeconds(10f); 
        Destroy(defender);
        Destroy(attacker);
        look.EnableCursor();
        SceneManager.LoadScene("LivingRoom");

    }

    private void CheckSound(string chosenAnimationAttacker)
    {
        switch (chosenAnimationAttacker)
        {
            case "isDying" :
                enemyDie.Play();
                break;
            case "isSlashing" :
                slash.Play();
                break;
            case "isBlocking" :
                block.Play();
                break;
            case "isDodging" :
                dodge.Play();
                break;
        }
    }

    void HandleDefenderTurn()
    {


        // Defender attacks
        string chosenAnimationDefender = "isSlashing";
        defenderAnimator.SetBool(chosenAnimationDefender, true);
        defenderAnimator.SetBool("isIdle", false);
        CheckSound(chosenAnimationDefender);

        // Attacker reacts
        string chosenAnimationAttacker = AnimationDecidingAttackerResponse();
        attackerAnimator.SetBool(chosenAnimationAttacker, true);
        attackerAnimator.SetBool("isIdle", false);
        CheckSound(chosenAnimationAttacker);

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
