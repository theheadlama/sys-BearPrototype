using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabInstantiate : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject bear;

    [SerializeField]
    private int numberOfBears;

    [SerializeField]
    private Material[] material;

    [SerializeField]
    private GameObject[] spawnPoints;
    
    void Start()
    {
        for (int i=0; i<numberOfBears; i++) {
            GameObject instantiatedChr = Instantiate(bear, spawnPoints[i].transform.position, spawnPoints[i].transform.rotation);
            var bearInstantiator = instantiatedChr.GetComponent<InitializeCharacter>();
            bearInstantiator.SetIndex(0);
            bearInstantiator.SetMaterial(material[i]);
            foreach(Transform child in bearInstantiator.transform) {
                child.GetComponent<Renderer>().material = material[i];
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
