using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class aimDownScope : MonoBehaviour 
{
	public RigidbodyFirstPersonController rbfpsController;
	[Header ("Camera settings")]
	public Camera cam;
	public Vector2 FieldOfViewRange = new Vector2 (60, 30);

	public MeshRenderer Gun;
	public Vector3 aimDownSight;
	public Vector3 hipfire;

	public Animator anim;
	public Animator ScopeOverlayAnim;

	public float TargetFieldOfView;
	public Vector2 FieldOfViewLimit = new Vector2 (10, 30);
	public float TargetFovSmoothing = 1;

	[Range (0, 10)]
	public float aimspeed = 5;

	public bool IsHipfire = true;
	public bool CanScope;

	public AimDownMode aimDownMode;
	public enum AimDownMode
	{
		toggle,
		hold
	}

	public float NormalRunMultiplier = 1.8f;

	public GameObject scopeOverlay;

	public float ScopeDelay = 1;

	public float HoldBreathTimeDuration;
	public float HoldBreathTimeRemaining;

	void Start ()
	{
		IsHipfire = true;
		TargetFieldOfView = 60;
		CanScope = true;
		HoldBreathTimeRemaining = HoldBreathTimeDuration;
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
				IsHipfire = false;

				if (CanScope == true) 
				{
					Invoke ("ScopeIn", ScopeDelay);
					TargetFieldOfView = 30;
				}
			}

			if (Input.GetMouseButton (1)) 
			{
				if (CanScope == true) 
				{
					if (IsInvoking ("ScopeIn") == false)
					{
						Invoke ("ScopeIn", ScopeDelay);
						IsHipfire = false;
					}
				}

				if (Input.GetKey (KeyCode.LeftShift) == false) 
				{
					if (HoldBreathTimeRemaining < HoldBreathTimeDuration)
					{
						HoldBreathTimeRemaining += Time.deltaTime;
					}
				}

			}

			if (Input.GetMouseButtonUp (1)) 
			{
				CanScope = true;
				IsHipfire = true;
				ScopeOut ();
			}
		}

		if (Input.GetKeyDown (KeyCode.LeftShift)) 
		{
			CanScope = false;
			//ForceHipFireMode ();
		}

		if (Input.GetKey (KeyCode.LeftShift))
		{
			//CanScope = false;

			if (Input.GetMouseButton (1)) 
			{
				CanScope = true;
				//ForceHipFireMode ();
			}
				
			if (HoldBreathTimeRemaining > 0)
			{
				ScopeOverlayAnim.SetBool ("HoldBreath", true);
				HoldBreathTimeRemaining -= Time.deltaTime;
			}

			if (HoldBreathTimeRemaining < 0) 
			{
				ScopeOverlayAnim.SetBool ("HoldBreath", false);
			}

			//ScopeOverlayAnim.Play ("ScopeOverlayHoldBreath");
	
		}

		if (Input.GetKeyUp (KeyCode.LeftShift))
		{
			CanScope = true;
			ScopeOverlayAnim.SetBool ("HoldBreath", false);

		}
		CheckAimDownSights ();

	}

	void CheckAimDownSights ()
	{
		// Make it aim down sights.
		if (IsHipfire == false)
		{
			rbfpsController.movementSettings.RunMultiplier = 1;

			if (IsInvoking ("AnimOn") == true)
			{
				CancelInvoke ("AnimOn");
			}

			anim.enabled = false;
			transform.localRotation = Quaternion.identity;
			transform.localPosition = Vector3.Slerp (transform.localPosition, aimDownSight, aimspeed * Time.deltaTime);

			// Scroll up is zooming in.
			if (Input.GetAxis ("Mouse ScrollWheel") > 0) 
			{
				ZoomIn ();
			}

			// Scroll down is zooming out.
			if (Input.GetAxis ("Mouse ScrollWheel") < 0) 
			{
				ZoomOut ();
			}
		}

		// Return to hip fire.
		if (IsHipfire == true)
		{
			rbfpsController.movementSettings.RunMultiplier = NormalRunMultiplier;

			if (IsInvoking ("AnimOn") == false)
			{
				Invoke ("AnimOn", 1);
			}

			transform.localPosition = Vector3.Slerp (transform.localPosition, hipfire, aimspeed * Time.deltaTime);
			TargetFieldOfView = 60;

			if (HoldBreathTimeRemaining < HoldBreathTimeDuration)
			{
				HoldBreathTimeRemaining += Time.deltaTime;
			}
		}

		cam.fieldOfView = Mathf.Lerp (cam.fieldOfView, TargetFieldOfView, TargetFovSmoothing * Time.deltaTime);
		TargetFieldOfView = Mathf.Clamp (TargetFieldOfView, FieldOfViewLimit.x, FieldOfViewLimit.y);
	}

	void ScopeIn ()
	{
		scopeOverlay.SetActive (true);
		Gun.enabled = false;
		//cam.fieldOfView = 30;
		//TargetFieldOfView = 30;
	}

	void AnimOn ()
	{
		anim.enabled = true;
	}

	void ScopeOut ()
	{
		CancelInvoke ("ScopeIn");
		anim.Play ("Idle");
		scopeOverlay.SetActive (false);
		Gun.enabled = true;
		cam.fieldOfView = FieldOfViewRange.x;
		IsHipfire = true;
	}

	void ZoomIn ()
	{
		TargetFieldOfView -= 5;
	}

	void ZoomOut ()
	{
		TargetFieldOfView += 5;
	}

	void ForceHipFireMode ()
	{
		// Force the hip fire mode.
		ScopeOut ();
	}
}
