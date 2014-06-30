/** Crane Controller lunafish 2014-06-30 **/
using UnityEngine;
using System.Collections;

public class LNCraneCtrl : MonoBehaviour {
	// anker
	public SkinnedMeshRenderer _anker = null; // crane arm 
	public GameObject _cargo = null; // attached cargo
	public Animator _anim; // crane animation controller

	// Use this for initialization
	void Start () {
		_anim.SetBool ("isMove", true); // test
	}

	// Update is called once per frame
	void Update () {

		// move cargo use crane arm
		if(_cargo)
			_cargo.transform.position = _anker.bounds.center + new Vector3(0.0f, -0.75f, 0.0f);
	}
}
