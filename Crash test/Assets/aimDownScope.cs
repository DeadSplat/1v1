using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class aimDownScope : MonoBehaviour 
{
	public RigidbodyFirstPersonController rbfpsController;
	public PlayerController playerControllerScript;

	[Header ("Camera settings")]
	public Camera cam;
	public Vector2 FieldOfViewRange = new Vector2 (60, 30);

	public MeshRenderer Gun;
	public Vector3 aimDownSight;
	public Vector3 hipfire;

	public Animator anim;

	public Animator ScopeOverlayAnim;
	public GameObject scopeOverlay;
	public float ScopeDelay = 1;

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

	[Header ("Hold Breath")]
	public float HoldBreathTimeDuration;
	public float HoldBreathTimeRemaining;

	[Header ("Reticule")]
	public RectTransform Reticule;
	public Vector3 ReticuleTargetScale = Vector3.one;
	public float ReticuleSizeSmoothing = 10;

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
					ReticuleTargetScale = Vector3.zero;
				}
			}

			if (Input.GetMouseButton (1)) 
			{
				

				if (Input.GetKey (KeyCode.LeftShift) == false) 
				{
					if (CanScope == true) 
					{
						if (IsInvoking ("ScopeIn") == false)
						{
							Invoke ("ScopeIn", ScopeDelay);
							IsHipfire = false;
						}
					}

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
			anim.SetBool ("IsSprinting", true);
			playerControllerScript.CameraRecoilAnim.SetBool ("IsHipfire", false);
			//ForceHipFireMode ();
		}

		if (Input.GetKey (KeyCode.LeftShift))
		{
			//CanScope = false;
			//anim.SetBool ("IsSprinting", true);
			//IsHipfire = true;
			//ReticuleTargetScale = new Vector3 (3.5f, 3.5f, 1);

			if (Input.GetMouseButton (1)) 
			{
				//ScopeOut ();

				/*if (IsHipfire == false)
				{
					CanScope = false;
					anim.SetBool ("IsSprinting", true);
				}*/
			
				//ForceHipFireMode ();
			}
				
			if (HoldBreathTimeRemaining > 0)
			{
				ScopeOverlayAnim.SetBool ("HoldBreath", true);
				HoldBreathTimeRemaining -= Time.deltaTime;
				playerControllerScript.CameraRecoilAnim.SetBool ("IsHipfire", true);
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
			anim.SetBool ("IsSprinting", false);
			ReticuleTargetScale = new Vector3 (1f, 1f, 1);

		}
		CheckAimDownSights ();
		UpdateReticuleSize ();

	}

	void UpdateReticuleSize ()
	{
		Reticule.localScale = Vector3.Lerp (Reticule.localScale, ReticuleTargetScale, ReticuleSizeSmoothing * Time.deltaTime);
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
		playerControllerScript.CameraRecoilAnim.SetBool ("IsHipfire",  false);

		scopeOverlay.SetActive (true);
		Gun.enabled = false;
		ReticuleTargetScale = Vector3.zero;

		//if (playerControllerScript.CameraRecoilAnim.GetCurrentAnimatorStateInfo (0).IsName ("CamScopeIdle") == false) 
		//{
		//	playerControllerScript.CameraRecoilAnim.Play ("CamScopeIdle");
		//}
		//cam.fieldOfView = 30;
		//TargetFieldOfView = 30;
	}

	void AnimOn ()
	{
		anim.enabled = true;
	}

	void ScopeOut ()
	{
		playerControllerScript.CameraRecoilAnim.SetBool ("IsHipfire", true);
		CancelInvoke ("ScopeIn");
		anim.Play ("Idle");
		scopeOverlay.SetActive (false);
		Gun.enabled = true;
		cam.fieldOfView = FieldOfViewRange.x;
		ReticuleTargetScale = Vector3.one;
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
