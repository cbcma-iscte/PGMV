using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;

public class XMLReader : MonoBehaviour
{

    [SerializeField]
    XmlDocument doc;
    // Start is called before the first frame update
    void Start()
    {
        if (doc != null)
        {
            IdentifyTag(doc.DocumentElement);
        }
        else
        {
            Debug.Log("XML Document is not assigned");
        }
    }

    void IdentifyTag(XmlNode node)
    {
        switch (node.Name)
        {
            case "game" :
                //invocar game
                return;
            case "role" :
                // invocar player class com info
                return;
            case "board" :
                // invocar funcao para dar render nos diferentes meshes do mapa.
                return;
            case "turn" :
                // invocar funcao que le o unit que esta la dentro e mete os argumentos (unit, role, type, action e position)
                return;
            default :
                return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
