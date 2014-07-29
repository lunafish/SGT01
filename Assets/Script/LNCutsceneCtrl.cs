/** Cutscene controler lunafish 2014-07-10 **/
using UnityEngine;
using SimpleJSON;
using System.Collections;

public class LNCutsceneCtrl : MonoBehaviour {
	public GameObject _npc; // NPC Sprite
	public GameObject _button; // Button Sprite
	public GameObject _buttonExit; // Button Sprite
	public GameObject _talk; // Talk Sprite
	public GameObject _next; // Next Button Sprite
	public GameObject _kiosk; // kiosk sprite

	// for state check
	static public bool _isEnable = false;


	// for json script
	private JSONNode _json = null;

	// Use this for initialization
	void Start () {
		_isEnable = false;
		// read json
		string txt;
		if( LNUtil.ReadText ("json/cutscene", out txt)) {
			_json = JSONNode.Parse( txt );
		}
		//
	}
	
	// Update is called once per frame
	void Update () {

	}

	// Enable
	public void Enable( string tag) {
		if(_json == null) {
			Debug.Log("No JSON");
			return;
		}

		JSONNode _node = _json [tag];
		if(_node == null) {
			Debug.Log("No Node : " + tag);
			return;
		}

		// check kiosk
		bool isKiosk = false;
		if(_node["kiosk"] != null) {
			isKiosk = true;
		}

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

		// set text
		_button.GetComponentInChildren<TextMesh> ().text = _node ["action"];
		_talk.GetComponentInChildren<TextMesh> ().text = _node ["talk"];
		_buttonExit.GetComponentInChildren<TextMesh> ().text = _node ["cancel"];
		if(_node["kiosk"] != null) _kiosk.GetComponentInChildren<TextMesh> ().text = _node["kiosk"];
		//

		_isEnable = true;
	}

	// Disable
	public void Disable( ) {
		_npc.SetActive (false);
		_kiosk.SetActive (false);
		_button.SetActive (false);
		_talk.SetActive (false);
		_next.SetActive (false);
		_buttonExit.SetActive (false);

		_isEnable = false;
	}

	// Exit button callback
	public void OnExit( ) {
		Disable();
	}

	// Action button callback
	public void OnAction( ) {
		LNUtil util = LNUtil.Instance();

		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		player.GetComponent<LNPlayerCtrl> ().Action();
	}
}
