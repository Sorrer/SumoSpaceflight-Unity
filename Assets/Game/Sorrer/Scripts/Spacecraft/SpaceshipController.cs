using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Todo - Test swapping physics simulation to deltaTime instead of fixed
//Or try just scrapping using rigidbody and create own type of velocity mover and if collide just stop moving?
//Or try just interpolating between the last point and the next point of the rigid body, that way it will stutter but not that hard.
//
//Try to aim the velocity more towards the current heading as a dampening effect for that arcadey feel
public class SpaceshipController : MonoBehaviour
{
	[Header("Settings")]
	public bool CanControl = true;
	public bool InvertedFlight;
	public Transform body;
	public Rigidbody rigidBody;

	public ParticleSystem[] particles;
	public TrailRenderer[] trails;


	[Space(4)]
	[Header("Camera Effect Settings")]
	public Camera cam;
	public Transform cam_min;
	public Transform cam_max;
	public GameObject AmbientParticles;

	[Space(4)]
	[Header("Rotation Settings")]
	[Range(0, 1)]
	public float CursorDeadzone;
	public AnimationCurve CursorDistanceCurve;
	[Range(0, 1)]
	public float MaxXRot;
	[Range(0, 1)]
	public float MaxYRot;
	public float RotationSpeed;
	public float ZRotationSpeed;
	public float RotationDampingSpeed;


	[Space(4)]
	[Header("Movement Settings")]
	[Range(0, 1)]
	public float VerticalSpeed;
	[Range(0, 1)]
	public float BackwardSpeed;
	public float MaxSpeed;
	public float MovementSpeed;

    // Start is called before the first frame update
    void Start()
    {
		Application.targetFrameRate = 300;
	}

    // Update is called once per frame
    void Update() {

		if (cam.gameObject.activeSelf) {
			cam.transform.localPosition = Vector3.Lerp(cam_min.localPosition, cam_max.localPosition, Mathf.Clamp((rigidBody.velocity.magnitude / MaxSpeed), 0, 1));
		}

	}

	private void FixedUpdate() {
		this.transform.position = body.position;
		this.transform.rotation = body.rotation;

		if (!CanControl) {
			cam.gameObject.SetActive(false);
			AmbientParticles.SetActive(false);
			return;
		}
		cam.gameObject.SetActive(true);

		UpdateRotation();


		UpdateMovement();
	}



	public void UpdateMovement() {



		Vector3 MovementVector = new Vector3();

		bool isForwardMoving = false;

		if (Input.GetKey(KeyCode.W)) {
			isForwardMoving = true;
			MovementVector += body.forward;
		}
		if (Input.GetKey(KeyCode.S)) {
			isForwardMoving = true;
			MovementVector -= body.forward * BackwardSpeed;
		}


		for(int i = 0; i < particles.Length; i++) {
			var emission = particles[i].emission;
			emission.enabled = isForwardMoving;
		}



		if (Input.GetKey(KeyCode.A)) {
			MovementVector -= body.right * VerticalSpeed;
		}
		if (Input.GetKey(KeyCode.D)) {
			MovementVector += body.right * VerticalSpeed;
		}


		if (Input.GetKey(KeyCode.X)) {
			MovementVector += body.up * VerticalSpeed;
		}

		if (Input.GetKey(KeyCode.Z)) {
			MovementVector -= body.up * VerticalSpeed;
		}

		MovementVector *= MovementSpeed;


		rigidBody.AddForce(MovementVector);
		float mag = rigidBody.velocity.magnitude;

		//print(mag);

		if(mag > MaxSpeed) {
			rigidBody.velocity = rigidBody.velocity.normalized * MaxSpeed;
		}

		if(mag > 4f) {
			for(int i = 0; i < trails.Length; i++) {
				trails[i].emitting = true;
			}
		} else {

			for (int i = 0; i < trails.Length; i++) {
				trails[i].emitting = false;
			}
		}

	}

	Vector3 currentRotation;

	void UpdateRotation() {

		Vector3 mousePos = Input.mousePosition;
		Vector3 mouseOffset = mousePos - GetCenterScreen();

		float dist = mouseOffset.magnitude;
		float maxRadius = GetMaxRadius();
		float distPercent = Mathf.Clamp(dist / maxRadius, 0, 1);

		

		mouseOffset.Normalize();
		mouseOffset *= CursorDistanceCurve.Evaluate(distPercent);

		mouseOffset.x *= MaxXRot;
		mouseOffset.y *= MaxYRot * (InvertedFlight ? -1 : 1);

		mouseOffset *= RotationSpeed;
		mouseOffset.z = 0;

		if (distPercent <= CursorDeadzone || !Application.isFocused) {
			mouseOffset.x = 0;
			mouseOffset.y = 0;
		}

		if (Input.GetKey(KeyCode.Q)) {
			mouseOffset.z -= ZRotationSpeed;
		}
		if (Input.GetKey(KeyCode.E)) {
			mouseOffset.z += ZRotationSpeed;
		}

		currentRotation = Vector3.Lerp(currentRotation, new Vector3(mouseOffset.y, mouseOffset.x, mouseOffset.z), RotationDampingSpeed * Time.fixedDeltaTime);

		this.body.Rotate(currentRotation * Time.fixedDeltaTime, Space.Self);
	}

	public Vector3 GetCenterScreen() {

		return new Vector3(Screen.width / 2, Screen.height / 2);

	}

	public float GetMaxRadius() {

		return (Screen.width > Screen.height ? Screen.height / 2 : Screen.width / 2);

	}

}
