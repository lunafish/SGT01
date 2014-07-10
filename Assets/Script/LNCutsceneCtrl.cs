/** Cutscene controler lunafish 2014-07-10 **/
using UnityEngine;
using System.Collections;

public class LNCutsceneCtrl : MonoBehaviour {
	public GameObject _npc; // NPC Sprite
	public GameObject _button; // Button Sprite
	public GameObject _buttonExit; // Button Sprite
	public GameObject _Talk; // Talk Sprite
	public GameObject _Next; // Next Button Sprite

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
		_Talk.SetActive (flag);
		_Next.SetActive (flag);
		_buttonExit.SetActive (flag);
	}

	// Exit button callback
	public void OnExit( ) {
		Active (false);
	}

	// Action button callback
	public void OnAction( ) {
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		player.GetComponent<LNPlayerCtrl> ().Action ();
	}
}
