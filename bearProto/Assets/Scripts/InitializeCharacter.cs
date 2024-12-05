using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InitializeCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    private int index;
    
    // index setter 
    public void SetIndex(int indexNumber) {
        index = indexNumber;
    }

    public void SetMaterial(Material material) {
        Renderer thisRenderer = GetComponent<Renderer>();
        thisRenderer.material = material;

    }


    void Awake() {

    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
