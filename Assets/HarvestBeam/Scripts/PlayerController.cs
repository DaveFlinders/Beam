using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public HarvestBeam harvestBeam;
    private float speed = 5f;
    private Transform targetToFace;

    private void Start()
    {
        HarvestBeam.onBeamActivated += activeBeam;
        HarvestBeam.onBeamDeactivated += deactiveBeam;
    }

    void activeBeam()
    {
        targetToFace = harvestBeam.Target;
    }

    void deactiveBeam()
    {
        targetToFace = null;
    }

    private void OnDisable()
    {
        HarvestBeam.onBeamActivated -= activeBeam;
        HarvestBeam.onBeamActivated -= deactiveBeam;
    }

    void Update () {
        Vector3 v3 = new Vector3(Input.GetAxisRaw("Horizontal"), 0.0f, Input.GetAxisRaw("Vertical"));

        if(targetToFace != null)
        {
            Vector3 lookAtVector = new Vector3(targetToFace.position.x, transform.position.y, targetToFace.position.z);
            transform.LookAt(lookAtVector);

        }
        else
        {
            if (v3 != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(v3);
        }

        transform.Translate(speed * v3.normalized * Time.deltaTime, Space.World);
    }
}
