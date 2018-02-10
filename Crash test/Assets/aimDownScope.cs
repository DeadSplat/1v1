using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class aimDownScope : MonoBehaviour 
{
	[Header ("Camera settings")]
	public Camera cam;
	public Vector2 FieldOfViewRange = new Vector2 (60, 30);

	public MeshRenderer Gun;
	public Vector3 aimDownSight;
	public Vector3 hipfire;

	[Range (0, 10)]
	public float aimspeed = 5;

	public bool IsHipfire = true;

	public AimDownMode aimDownMode;
	public enum AimDownMode
	{
		toggle,
		hold
	}

	public GameObject scopeOverlay;

	public float ScopeDelay = 1;



	void Start ()
	{
		IsHipfire = true;
	}

	void Update () 
	{
		if (aimDownMode == AimDownMode.toggle)
		{
			if (Input.GetMouseButtonDown (1)) 
			{
				IsHipfire = !IsHipfire;
			}
		}

		if (aimDownMode == AimDownMode.hold) 
		{
			if (Input.GetMouseButtonDown (1)) 
			{
				Invoke ("ScopeIn", ScopeDelay);
			}

			if (Input.GetMouseButton (1)) 
			{
				IsHipfire = false;
			}

			if (Input.GetMouseButtonUp (1)) 
			{
				IsHipfire = true;
				ScopeOut ();
			}
		}

		CheckAimDownSights ();
	}

	void CheckAimDownSights ()
	{
		// Make it aim down sights.
		if (IsHipfire == false)
		{
			transform.localPosition = Vector3.Slerp (transform.localPosition, aimDownSight, aimspeed * Time.deltaTime);
		}

		// Return to hip fire.
		if (IsHipfire == true)
		{
			transform.localPosition = Vector3.Slerp (transform.localPosition, hipfire, aimspeed * Time.deltaTime);
		}
	}

	void ScopeIn ()
	{
		scopeOverlay.SetActive (true);
		Gun.enabled = false;
		cam.fieldOfView = FieldOfViewRange.y;
	}

	void ScopeOut ()
	{
		CancelInvoke ("ScopeIn");
		scopeOverlay.SetActive (false);
		Gun.enabled = true;
		cam.fieldOfView = FieldOfViewRange.x;
	}
}
