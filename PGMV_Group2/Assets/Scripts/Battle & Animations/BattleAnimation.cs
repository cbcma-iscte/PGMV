using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


/// <summary>
/// Handles the battle animation between the attacker and defender.
/// Manages movement, attack, defense, and the state transitions between them.
/// </summary>
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

    static public int _TERRAIN_SCALE = 1;

    /// <summary>
    /// Initializes the battle animation by setting the initial positions of the attacker and defender.
    /// </summary>
    void Start()
    {
        int heightMiddle = terrain.terrainData.heightmapResolution / 2;
        attacker.transform.position = new Vector3(attacker.transform.position.x, terrain.terrainData.GetHeight(heightMiddle, heightMiddle) * _TERRAIN_SCALE + 20 , attacker.transform.position.z);
        defender.transform.position = new Vector3(defender.transform.position.x ,terrain.terrainData.GetHeight(heightMiddle, heightMiddle) * _TERRAIN_SCALE + 20 , defender.transform.position.z);
        attackerAnimator = attacker.GetComponent<Animator>();
        defenderAnimator = defender.GetComponent<Animator>();
    }

    /// <summary>
    /// Deactivates all animation booleans for both attacker and defender.
    /// </summary>
    void DeactivateAllAnimations()
    {
        attackerAnimator.SetBool("isSlashing", false);
        attackerAnimator.SetBool("isBlocking", false);
        attackerAnimator.SetBool("isDodging", false);

        defenderAnimator.SetBool("isSlashing", false);
        defenderAnimator.SetBool("isBlocking", false);
        defenderAnimator.SetBool("isDodging", false);
    }
 
    /// <summary>
    /// Sets the isGrounded flag when the attacker collides with the ground.
    /// </summary>
    /// <param name="collision">The collision data associated with the ground collision.</param>
   private void OnCollisionEnter(Collision collision)
    {
        isGrounded = true;
    }

    /// <summary>
    /// Handles the state transitions and updates based on the current state of the battle.
    /// </summary>
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

    /// <summary>
    /// Handles the movement of the attacker towards the defender.
    /// </summary>
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

    /// <summary>
    /// Handles the attacker's turn, including its attack and the defender's reaction.
    /// </summary>
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

    /// <summary>
    /// Coroutine to leave the scene and return to the main menu.
    /// </summary>
    /// <returns>IEnumerator for the coroutine.</returns>
    IEnumerator LeaveScene()
    {

        yield return new WaitForSeconds(10f); 
        Destroy(defender);
        Destroy(attacker);
        look.EnableCursor();
        GameObject[] menu = GameObject.FindGameObjectsWithTag("MenuInformation");
        if (menu.Length > 0)
        {
            menu[0].SetActive(true);
        }
        SceneManager.LoadScene("MainMenu");

    }

    /// <summary>
    /// Plays the appropriate sound effect based on the animation.
    /// </summary>
    /// <param name="chosenAnimation">The animation being played.</param>
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

    /// <summary>
    /// Handles the defender's turn, including its attack and the attacker's reaction.
    /// </summary>
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

    /// <summary>
    /// Randomly decides the defender's animation.
    /// </summary>
    /// <returns>The chosen defender animation.</returns>
    string AnimationDecidingDefender()
    {
        int rand = Random.Range(0, allAnimationDefender.Length);
        return allAnimationDefender[rand];
    }

    /// <summary>
    /// Randomly decides the attacker's response animation.
    /// </summary>
    /// <returns>The chosen attacker animation.</returns>
    string AnimationDecidingAttackerResponse()
    {
        int rand = Random.Range(0, allAnimationAttacker.Length);
        return allAnimationAttacker[rand];
    }
}
