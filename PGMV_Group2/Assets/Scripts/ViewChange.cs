using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// The ViewChange class handles the functionality of changing the camera view
/// between a default position and tracking a selected table or board object.
/// </summary>
public class ViewChange : MonoBehaviour{
    private bool view_change = false;
    public GameObject[] Tables_Boards_ToLook;
    private GameObject trackedObject;
    public GameObject miniMapCamera;
    private GameObject gamePlaying;

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// Prevent the miniMapCamera and the main camera from being destroyed when loading a new scene.
    /// </summary>
    void Awake(){
        DontDestroyOnLoad(miniMapCamera);
        DontDestroyOnLoad(Camera.main);
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// Sets the minimap camera's position and rotation to match the main camera.
    /// Finds all tables existing in the scene
    /// </summary>
    void Start()
    {  
        //transform.position = new Vector3((float)-0.37,(float)3.64,(float)-8.16);
        miniMapCamera.transform.position = transform.position;
        miniMapCamera.transform.rotation = transform.rotation;

        miniMapCamera.SetActive(false);
        if(Tables_Boards_ToLook.Length == 0){  
            Tables_Boards_ToLook = GameObject.FindGameObjectsWithTag("Table");
            trackedObject = Tables_Boards_ToLook[0];}
          
    }   
        
    /// <summary>
    /// Changes the camera view.
    /// </summary>
    /// <param name="view_change">Indicates whether to switch to tracking view or default view.</param>
    void changeView(bool view_change){
        if (!view_change){
            if(gamePlaying!=null)gamePlaying.GetComponent<GameManager>().showPontuations(false);
            miniMapCamera.SetActive(false);
            
            transform.parent = null;
            transform.position = new Vector3((float)-0.37,(float)3.64,(float)-8.16);
            transform.localRotation = Quaternion.Euler((float)4.96,(float)0,(float)0);    
        }else{
            
            miniMapCamera.SetActive(true);

            transform.parent = trackedObject.transform;

            transform.position = new Vector3(trackedObject.transform.position.x,(float)3.5,trackedObject.transform.position.z);
            transform.localPosition = new Vector3(0,3.31999993f,-0.410000012f);

            transform.LookAt(trackedObject.transform);
            Quaternion targetRotation = Quaternion.Euler(transform.eulerAngles.x, trackedObject.transform.eulerAngles.y, transform.eulerAngles.z);
            
            transform.rotation = targetRotation;

            GameObject[] gamesPlaying = GameObject.FindGameObjectsWithTag("GameController");
            if(gamesPlaying.Length!=0){
                gamePlaying = gamesPlaying[0];
                gamePlaying.GetComponent<GameManager>().showPontuations(true);
            }else{
                gamePlaying=null;
            }


                
            
        }  
    }

     /// <summary>
    /// Update is called once per frame.
    /// Verifies if the mouse is pressed to change the view to it
    /// </summary>
    void Update()
    {

        if (Input.GetMouseButtonDown(0)) //left-size of the mouse
        {
            //This creates a raycast from the mouse and see if it hits something more specifically a board with a table that is in the array
            Ray raycast = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(raycast, out hit,Mathf.Infinity) && hit.collider!=null)
            {                
                foreach (GameObject table_board in Tables_Boards_ToLook)
                {
                    if (hit.collider.gameObject == table_board){
                        trackedObject = table_board;     
                        view_change=!view_change;
                        changeView(view_change);
                        
                    }
                }
                
            }
        }
       
        
        
    }
}
