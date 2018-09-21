using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour {

	public RingController bigRing, midRing, smallRing;
	public NailPoint nailPoint;

	public static SceneObject instance;
	// Use this for initialization
	void Start () {
		instance = this;
	}

	// Update is called once per frame
	void Update () {

	}
}