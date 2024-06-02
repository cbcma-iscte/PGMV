using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battle : MonoBehaviour
{

    [SerializeField] private GameObject soldierPrefab;
    [SerializeField] private GameObject terrain;

    GameObject soldierAttacking;

    GameObject soldierDefending;

    void Awake()
    {
        
    }

    public void SpawnAttackingSoldier(float width, float height, float length)
    {


        Debug.Log("Spawning attacking soldier");
        soldierAttacking = Instantiate(soldierPrefab, terrain.transform);
        soldierAttacking.transform.localPosition = new Vector3(width, height + 20, length);
    }

    public void SpawnDefendingSoldier(float width, float height, float length)
    {

        Debug.Log("Spawning defending soldier");
        soldierDefending = Instantiate(soldierPrefab, terrain.transform);
        soldierDefending.transform.localPosition = new Vector3(width, height+20, length);
    }

    public void StartBattle()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
