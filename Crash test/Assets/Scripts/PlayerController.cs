using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerController : MonoBehaviour
{
	public GameObject bullet;
	public Transform BulletSpawn;
	public float FireRate = 0.1f;
	private float nextFire;

	public ParticleSystem MuzzleFlash;

	void Update ()
	{
		if (Input.GetMouseButton (0) && Time.time > nextFire) 
		{
			Shoot ();
			nextFire = Time.time + FireRate;
		}

		if (Input.GetKeyDown (KeyCode.Escape)) 
		{
			Application.Quit ();
		}
	}

	void Shoot ()
	{
		Instantiate (bullet, BulletSpawn.position, BulletSpawn.rotation);
		MuzzleFlash.Play ();
	}
}
