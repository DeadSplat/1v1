using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetFrameRate : MonoBehaviour 
{
	public int framerate = -1;

	void Awake ()
	{
		Application.targetFrameRate = framerate;
	}

}
