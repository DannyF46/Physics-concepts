using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FollowCoM : MonoBehaviour
{
    [SerializeField] public bool followCenterOfMass;
    [SerializeField] private CoM cm;
    private Camera cam;
    // Start is called before the first frame update
    void Start()
    {
         cam = this.GetComponent<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(followCenterOfMass)
        {

            cam.transform.position = new Vector3(cm.CenterofMass.x, cm.CenterofMass.y,0) + 5*Vector3.back;

        }
        
    }


    
}
