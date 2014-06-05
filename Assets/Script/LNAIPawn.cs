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

	// Use this for initialization
	void Start () {
		Emotion (eEMOTION.NONE);
	}
	
	// Update is called once per frame
	void Update () {
		Update_state ();
	}

	// update state machine
	void Update_state( ) {
		if(_current_state == eSTATE.STAY) {
			state_stay( );
		} else if(_current_state == eSTATE.DAMAGE ) {
			state_damage( );
		}
	}

	// state machine
	void state_stay( ) {

	}

	void state_damage( ) {
		_state_delta += Time.deltaTime;

		// check exit
		if(_state_delta > _stun_delta) {
			Emotion (eEMOTION.NONE);
			change_state( eSTATE.STAY );
		}
	}
	//

	void Emotion( eEMOTION e ) {
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

		Emotion (eEMOTION.SMASH);
		change_state (eSTATE.DAMAGE);
	}

}
