using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
	public GameObject bullet;
	public Transform BulletSpawn;
	public Transform BulletSpawnScoped;
	public float FireRate = 0.1f;
	private float nextFire;

	public ParticleSystem MuzzleFlash;
	public Animator GunAnim;
	public Animator CameraRecoilAnim;

	public aimDownScope aimScript;

	public Transform CamPos;
	public float NormalHeight = 0.6f;
	public float CrouchHeight = 0.2f;
	public float CrouchSmoothingTime = 1;
	public float BulletInaccuracyFactor = 1;

	void Update ()
	{
		if (Input.GetMouseButton (0) && Time.time >= nextFire) 
		{
			Shoot ();
			nextFire = Time.time + FireRate;
		}

		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			Application.Quit ();
		}

		if (Input.GetKey (KeyCode.LeftControl) == true) 
		{
			CamPos.localPosition = new Vector3 (
				0, 
				Mathf.Lerp (CamPos.localPosition.y, CrouchHeight, CrouchSmoothingTime * Time.deltaTime), 
				0
			);

			if (aimScript.IsHipfire == true)
			{
				aimScript.ReticuleTargetScale = new Vector3 (0.5f, 0.5f, 0.5f);
			}

			if (aimScript.IsHipfire == false)
			{
				aimScript.ReticuleTargetScale = new Vector3 (0.0f, 0.0f, 0.0f);
			}
		}

		if (Input.GetKey (KeyCode.LeftControl) == false) 
		{
			CamPos.localPosition = new Vector3 (
				0, 
				Mathf.Lerp (CamPos.localPosition.y, NormalHeight, CrouchSmoothingTime * Time.deltaTime), 
				0
			);
		}

		if (Input.GetKeyUp (KeyCode.LeftControl)) 
		{
			aimScript.ReticuleTargetScale = new Vector3 (1f, 1f, 1f);
		}

	}

	void Shoot ()
	{
		aimScript.anim.enabled = true;
		MuzzleFlash.Play ();

		if (aimScript.IsHipfire == true)
		{
			aimScript.Reticule.localScale = new Vector3 (2f, 2, 2f);
			Quaternion BulletSpawnInaccuracy = BulletSpawn.rotation * Quaternion.Euler(Random.insideUnitSphere * BulletInaccuracyFactor);
			Instantiate (bullet, BulletSpawn.position, BulletSpawnInaccuracy);
			GunAnim.Play ("Recoil");
		}

		if (aimScript.IsHipfire == false) 
		{
			aimScript.Reticule.localScale = new Vector3 (0f, 0, 0f);
			Instantiate (bullet, BulletSpawnScoped.position, BulletSpawnScoped.rotation);
			CameraRecoilAnim.Play ("CamRecoil");


			aimScript.ScopeOverlayAnim.Play ("ScopeOverlayShoot");
		}
	}
}
