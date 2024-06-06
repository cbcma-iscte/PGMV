using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

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
    [SerializeField]float gravity;
    [SerializeField] Terrain terrain;
    [SerializeField] AudioSource walk;
    [SerializeField] AudioSource jump;


    void Start()
    {
        int heightMiddle = terrain.terrainData.heightmapResolution;
        player.transform.position = new Vector3( player.transform.position.x,terrain.terrainData.GetHeight(heightMiddle, heightMiddle) , player.transform.position.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (cc.isGrounded)
        {

            ccAnimator.SetBool("isJumping",false);

             // Get the camera's forward and right vectors
            Vector3 camForward = Camera.main.transform.forward;
            Vector3 camRight = Camera.main.transform.right;
            camForward.y = 0f;
            camRight.y = 0f;
            camForward.Normalize();
            camRight.Normalize();

            // Obtém a direção do movimento
            float moveHorizontal = Input.GetAxis("Horizontal");
            float moveVertical = Input.GetAxis("Vertical");

            // Obter direção do movimento
            direction = (camForward * moveVertical + camRight * moveHorizontal).normalized;

            if (direction.magnitude > 0.1f)
            {
            
                // Determina a velocidade do movimento
                float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? speed * runMultiplier : speed;
                direction *= currentSpeed;

                if (!walk.isPlaying)
                    walk.Play();

                // Definindo o parâmetro de movimento do animator
                ccAnimator.SetBool("isIdle",false);
                ccAnimator.SetFloat("MoveX", moveHorizontal);
                ccAnimator.SetFloat("MoveZ", moveVertical);
                ccAnimator.SetBool("isWalking",true);
                ccAnimator.SetBool("isRunning",Input.GetKey(KeyCode.LeftShift));
            }
            else
            {
                ccAnimator.SetBool("isIdle",true);
                ccAnimator.SetBool("isRunning",false);
                ccAnimator.SetBool("isWalking",false);
                ccAnimator.SetFloat("MoveX", 0);
                ccAnimator.SetFloat("MoveZ", 0);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                jump.Play();
                direction.y = jumpSpeed;
                direction.x = 0;
                direction.z = 0;
                ccAnimator.SetBool("isJumping",true);
                
            }
        }
        // Aplicar gravidade
        direction.y -= gravity * Time.deltaTime;

        // Mover o character
        cc.Move(direction * Time.deltaTime);
    }

    private void LateUpdate()
    {
        look.ProcessLook(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
    }
}
