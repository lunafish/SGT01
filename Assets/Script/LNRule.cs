using UnityEngine;
using System.Collections;

public class LNRule : MonoBehaviour {
	private GameObject[] _pawns;
	private GameObject[] _players;

	// Find type
	public enum TYPE {
		ENEMY = 0,
		NPC,
		PLAYER,
	};


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

		// make player list
		_players = GameObject.FindGameObjectsWithTag ("Player");
	}

	// reset
	public void UpdatePawnList() { 
		// make pawn list
		_pawns = GameObject.FindGameObjectsWithTag ("Pawn");
	}

	// Find enemy
	GameObject findTargetEnemy( GameObject source ) {
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
				
				//_pawns[i].GetComponent<LNPawn>().Target( null ); // clear target
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

	// Find player
	GameObject findTargetPlayer( GameObject source ) {
		// now only one player
		GameObject player = _players[0];

		float len = source.GetComponent<LNPawn> ()._sight_length;
		Vector3 v = player.transform.position - source.transform.position;
		if(v.magnitude < len) {
			// find player
			return player;
		}

		return null;
	}

	// Find Target
	public GameObject FindTarget( GameObject source, TYPE type = TYPE.ENEMY ) {
		GameObject target = null;
		if(type == TYPE.PLAYER) {
			target = findTargetPlayer( source );
		} else {
			target = findTargetEnemy( source );
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
