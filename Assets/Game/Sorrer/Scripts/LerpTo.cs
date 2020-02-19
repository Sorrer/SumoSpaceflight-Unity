using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LerpTo : MonoBehaviour
{

	public float LerpSpeed;
	public Transform lerpTo;

    // Update is called once per frame
    void Update()
    {
		this.transform.position = Vector3.Lerp(this.transform.position, lerpTo.position, LerpSpeed * Time.deltaTime);

    }
}
