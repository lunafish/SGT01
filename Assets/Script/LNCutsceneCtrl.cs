/** Cutscene controler lunafish 2014-07-10 **/
using UnityEngine;
using System.Collections;

public class LNCutsceneCtrl : MonoBehaviour {
	public GameObject _npc; // NPC Sprite
	public GameObject _button; // Button Sprite
	public GameObject _buttonExit; // Button Sprite
	public GameObject _talk; // Talk Sprite
	public GameObject _next; // Next Button Sprite
	public GameObject _kiosk; // kiosk sprite

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Enable
	public void Enable( bool isKiosk ) {
		// kiosk mode?
		if(isKiosk) {
			_npc.SetActive (false);
			_kiosk.SetActive (true);
		} else {
			_npc.SetActive (true);
			_kiosk.SetActive (false);
		}
		_button.SetActive (true);
		_talk.SetActive (true);
		_next.SetActive (true);
		_buttonExit.SetActive (true);
	}

	// Disable
	public void Disable( ) {
		_npc.SetActive (false);
		_kiosk.SetActive (false);
		_button.SetActive (false);
		_talk.SetActive (false);
		_next.SetActive (false);
		_buttonExit.SetActive (false);
	}

	// Exit button callback
	public void OnExit( ) {
		Disable();
	}

	// Action button callback
	public void OnAction( ) {
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		player.GetComponent<LNPlayerCtrl> ().Action ();
	}
}
