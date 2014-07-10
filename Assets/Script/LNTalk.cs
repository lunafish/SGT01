using UnityEngine;
using System.Collections;

public class LNTalk : MonoBehaviour {
	public GameObject _npc;
	public GameObject _button;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// active or deactive
	public void Active( bool flag ) {
		_npc.SetActive (flag);
		_button.SetActive (flag);
	}
}
