using UnityEngine;
using System.Collections;

public class LNRule : MonoBehaviour {
	private GameObject[] _pawns;

	// Use this for initialization
	void Start () {
		InitPawn ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Init Pawn
	void InitPawn( ) {
		// make dynamic pawns

		// make pawn list
		_pawns = GameObject.FindGameObjectsWithTag ("Pawn");
	}

	// reset
	public void UpdatePawnList() { 
		// make pawn list
		_pawns = GameObject.FindGameObjectsWithTag ("Pawn");
	}

	// Find Target
	public GameObject FindTarget( GameObject source ) {
		float len = source.GetComponent<LNPawn> ()._sight_length;
		Vector3 v;
		GameObject target = null;
		bool update = false;
		for(int i = 0; i < _pawns.Length; i++) {
			// check null pawns (for update)
			if(_pawns[i] == null) {
				update = true;
				continue;
			}

			if(source != _pawns[i]) {
				if(_pawns[i].GetComponent<LNPawn>() == null) {
					Debug.Log("ERROR : " + _pawns[i].name + " " + _pawns[i].GetComponent<LNAIPawn>());
				}

				_pawns[i].GetComponent<LNPawn>().Target( null ); // clear target
				v = _pawns[i].transform.position - source.transform.position;
				float t = v.magnitude;
				if( t < len ) {
					v.Normalize();
					float dot = Vector3.Dot( source.GetComponent<LNPawn>()._avatar.transform.forward, v); // dot product (is forward sight?)
					if(dot > 0.33f) {
						target = _pawns[i];
						len = t;
					}
				}
			}
		}

		if(target) {
			target.GetComponent<LNPawn>().Target( source ); // set source target
		}

		// update pawn list
		if(update) {
			UpdatePawnList();
		}

		return target;
	}

	// attack under control
	public void Attack( GameObject source, GameObject target ) {
		// attack process

		//
		target.GetComponent<LNPawn> ().Damage (source);
	}

	// action (NPC)
	public void Action( GameObject source, GameObject target ) {
		// action process

		//
		target.GetComponent<LNPawn> ().Talk (source);
	}
}
