using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockRotation : MonoBehaviour
{

	public Transform LockTo;


    // Update is called once per frame
    void Update()
    {
		this.transform.rotation = LockTo.rotation;
    }
}
