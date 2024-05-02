using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn 
{
    //contains units that are a class that contains all the info that will happen
    public int Id;
    public List<Unit> Units { get; set; }
    
    public Turn(int id)
    {
        Id = id;
        Units = new List<Unit>();
    }
    
}
