using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LNTutorialCtrl : MonoBehaviour {
	// Use this for initialization
	void Start () {
		if (!LNUtil.Instance ()._isTutorial) {
			gameObject.SetActive (false);
		}
	}
	
	// Update is called once per frame
	void Update () {
		// any key input tutorial off
		if (Input.anyKey) {
			LNUtil.Instance ()._isTutorial = false;
			gameObject.SetActive (false);
		}
	}
}
