using UnityEngine;
using System.Collections;

public class LNAIPawn : LNPawn {
	public float _stun_delta = 1.0f;
	// emotion
	public GameObject[] _emotions = new GameObject[(int)eEMOTION.MAX]; // emotion sprite
	eEMOTION _current_emotion = eEMOTION.ALERT; // set not init value
	enum eEMOTION {
		ALERT = 0,
		TARGET,
		TALK,
		SMASH,
		MISS,
		NONE,
		MAX,
	};

	// NPC
	public enum eNPC {
		INFO = 0,
		STORE,
		GATE,
		EVENT,
	};
	public eNPC _npc = eNPC.INFO;

	// Use this for initialization
	void Start () {
		Emotion (eEMOTION.NONE);
	}
	
	// Update is called once per frame
	void Update () {
		Update_state ();
		update_emotion ();

	}

	void update_emotion () {
		// look at main camera
		for(int i = 0; i < _emotions.Length; i++) {
			if(_emotions[i] != null) {
				Vector3 v = Camera.main.transform.position;
				v.y = _emotions[i].transform.position.y;
				_emotions[i].transform.LookAt( v );
			}
		}
	}

	// update state machine
	void Update_state( ) {
		if(_current_state == eSTATE.STAY) {
			state_stay( );
		} else if(_current_state == eSTATE.DAMAGE ) {
			state_damage( );
		} else if(_current_state == eSTATE.TALK ) {
			state_talk( );
		}
	}

	// state machine
	void state_stay( ) {
		// npc talk check
		if(_type == ePawn.NPC) {
			if(_target) {
				Vector3 v = _target.transform.position - transform.position;
				if(v.magnitude < _short_attack_range) {
					Emotion(eEMOTION.TALK);
					change_state(eSTATE.TALK);
				}
			}
		}
	}

	void state_damage( ) {
		_state_delta += Time.deltaTime;

		// check exit
		if(_state_delta > _stun_delta) {
			Emotion (eEMOTION.NONE);
			change_state( eSTATE.STAY );
		}
	}

	// talk state(NPC)
	void state_talk( ) {
		if(_target) {
			Vector3 v = _target.transform.position - transform.position;
			if(v.magnitude > _short_attack_range) {
				//Emotion(eEMOTION.NONE);
				change_state(eSTATE.STAY);
			}
		}
		else {
			//Emotion(eEMOTION.NONE);
			change_state(eSTATE.STAY);
		}
	}
	//

	void Emotion( eEMOTION e ) {
		if(_current_state == eSTATE.TALK)
			return; // talk state don't change emotion

		if(_current_emotion == e )
			return;

		for(int i = 0; i < (int)eEMOTION.MAX; i++) {
			// set emotion
			if(e == (eEMOTION)i)
				_current_emotion = e;

			if(_emotions != null) {
				if(e == (eEMOTION)i ) {
					// correct emotion!
					_emotions[i].SetActive(true);
				}
				else {
					_emotions[i].SetActive(false);
				}
			}
		}
	}

	// callback

	// targeting call back
	public override void Target(GameObject target) {
		// only for stay state
		if(_current_state != eSTATE.STAY)
			return;

		base.Target (target); // call parents method
		if(target == null) {
			Emotion (eEMOTION.NONE);
		} else {
			Emotion (eEMOTION.TARGET);
		}
	}

	// damage call back
	public override void Damage(GameObject source) {
		if(_current_state == eSTATE.DAMAGE)
			return;

		// knock back
		Vector3 v = transform.position - source.transform.position;
		v.Normalize ();
		v.y = 0.0f;
		v *= 0.25f;
		Move (v);
		//

		Emotion (eEMOTION.SMASH);
		change_state (eSTATE.DAMAGE);
	}

	// get Talk from source
	public override void Talk(GameObject source) {
		// testcode
		if(_npc == eNPC.GATE) {
			Debug.Log("GATE");
			Application.LoadLevel("Dungeon");
		}
	}


	// move
	bool Move( Vector3 mov ) {
		RaycastHit hit;
		Vector3 margin = new Vector3 (0.0f, 0.5f, 0.0f);

		BoxCollider box = GetComponent<BoxCollider> ();
		bool bMax = Physics.Raycast (box.bounds.max + mov + margin, -Vector3.up, out hit);
		bool bMin = Physics.Raycast (box.bounds.min + mov + margin, -Vector3.up, out hit);
		
		if ((bMax == true) && (bMin == true)) {
			transform.position += mov;
		} else {
			return false;
		}

		return true;
	}

}
