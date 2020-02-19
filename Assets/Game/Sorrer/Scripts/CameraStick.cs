using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStick : MonoBehaviour
{

	public LayerMask Ignore;

	public Transform center;
	public Transform fcamera;
	public Transform cameraGoal;
    // Update is called once per frame
    void Update()
    {

		Vector3 dir = cameraGoal.position - center.position;
		Ray ray = new Ray(center.position, dir.normalized);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit, dir.magnitude, Ignore.value)) {

			fcamera.localPosition = hit.point - fcamera.parent.position;

		} else {
			//fcamera.position = cameraGoal.position;
		}


    }
}
