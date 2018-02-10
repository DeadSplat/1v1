using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour 
{
	public Rigidbody rb;
	public float speed = 50;
	public float lifetime = 2;
	public string ParentObjectName = "InstantiatedBullets";

	void Start ()
	{
		transform.parent = GameObject.Find (ParentObjectName).transform;

		rb = GetComponent<Rigidbody> ();
		//rb.velocity = transform.InverseTransformDirection (new Vector3 (0, 0, speed));
		//rb.velocity = new Vector3 (0, 0, speed);
		rb.velocity = transform.TransformDirection (new Vector3 (0, 0, speed));

		Destroy (gameObject, lifetime);
	}

}
