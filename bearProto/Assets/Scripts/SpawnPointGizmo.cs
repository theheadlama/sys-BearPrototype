using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;


public class SpawnPointGizmo : MonoBehaviour
{

    [SerializeField]
    private Color gizmoColor = new Color(255,255,255);

    void OnDrawGizmos(){
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(transform.position,0.25f);
    }
    

}
