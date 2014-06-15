using UnityEngine;
using System.Collections;

/**
 *  Dungeon Gear Control Script
 */

public class LNGearCtrl : MonoBehaviour {
	public float _speed = 1.0f; // Gear Rotate Speed

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (0.0f, Time.deltaTime * _speed, 0.0f); // rotate gear
	
	}
}
