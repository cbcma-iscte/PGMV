using UnityEngine;

/// <summary>
/// Controls the player character's movement and interactions, including walking, running, jumping, and camera control.
/// </summary>
public class PlayerController : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private CharacterController cc;
    [SerializeField] private Animator ccAnimator;
    [SerializeField] public CameraController look;
    private Vector3 direction;
    [SerializeField] float speed;
    [SerializeField] float runMultiplier;
    [SerializeField] float jumpSpeed;
    [SerializeField] float gravity;
    [SerializeField] Terrain terrain;
    [SerializeField] AudioSource walk;
    [SerializeField] AudioSource jump;

    static public int _TERRAIN_SCALE = 1;

    /// <summary>
    /// Initializes the player position based on the terrain height.
    /// </summary>
    void Start()
    {
        int heightMiddle = terrain.terrainData.heightmapResolution / 2;
        player.transform.position = new Vector3(player.transform.position.x, terrain.terrainData.GetHeight(heightMiddle, heightMiddle) * _TERRAIN_SCALE + 20, player.transform.position.z);
    }

    /// <summary>
    /// Updates the player's movement and animations each frame.
    /// </summary>
    void Update()
    {

        if (cc.isGrounded)
        {

            ccAnimator.SetBool("isJumping", false);

            // Get the camera's forward and right vectors
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // Get the direction of the movement
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            // Calculate the direction of the movement
            direction = (camForward * moveVertical + camRight * moveHorizontal).normalized;

            if (direction.magnitude > 0.1f)
            {

                // Determine the speed of the movement
                float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * runMultiplier : speed;
                direction *= currentSpeed;

                if (!walk.isPlaying)
                    walk.Play();

                // Set animator parameters for movement
                ccAnimator.SetBool("isIdle", false);
                ccAnimator.SetFloat("MoveX", moveHorizontal);
                ccAnimator.SetFloat("MoveZ", moveVertical);
                ccAnimator.SetBool("isWalking", true);
                ccAnimator.SetBool("isRunning", Input.GetKey(KeyCode.LeftShift));
            }
            else
            {
                // Set animator parameters for idle
                ccAnimator.SetBool("isIdle", true);
                ccAnimator.SetBool("isRunning", false);
                ccAnimator.SetBool("isWalking", false);
                ccAnimator.SetFloat("MoveX", 0);
                ccAnimator.SetFloat("MoveZ", 0);
            }

            // Handle jumping
            if (Input.GetKey(KeyCode.Space))
            {
                jump.Play();
                direction.y = jumpSpeed;
                direction.x = 0;
                direction.z = 0;
                ccAnimator.SetBool("isJumping", true);

            }
        }
        // Apply gravity
        direction.y -= gravity * Time.deltaTime;

        // Move the character
        cc.Move(direction * Time.deltaTime);
    }

    /// <summary>
    /// Processes the camera look input in LateUpdate to ensure smooth camera movement.
    /// </summary>
    private void LateUpdate()
    {
        look.ProcessLook(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
    }
}