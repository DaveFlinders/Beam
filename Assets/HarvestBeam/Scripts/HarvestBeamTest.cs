using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestBeamTest : MonoBehaviour {

    public HarvestBeam harvestBeam;
    public Transform target;
    public float maxDistanceFromNode = 5f;
    public PlayerController playerController;

    void Update () {

		if(target != null)
        {

            // this is a testing code block which calls "ActivateHarvestBeam" within a certain distance
            float distanceFromNode = Mathf.Abs(Vector3.Distance(transform.position, target.position));

            if(distanceFromNode < maxDistanceFromNode)
            {
                harvestBeam.effectSpeed += Time.deltaTime; // simulate tap
                if(harvestBeam.effectSpeed > 20f)
                {
                    harvestBeam.DeactivateHarvestBeam();
                }

                harvestBeam.UpdateHarvestBeam(target);

            } else
            {
                harvestBeam.DeactivateHarvestBeam();
            }
        }
	}
}
