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
	public string _npc_target = "Dungeon";
	public string _npc_cutscene = "gameshop"; 

	private LNRule _rule; // rule ctrl;

	private GameObject _cutscene = null; // hud
	private LNUIBarCtrl _battery = null; // status bar
	private GameObject _empty = null; // HP 0

	// Use this for initialization
	void Start () {
		Emotion (eEMOTION.NONE);
		// get rule book and action target
		_rule = GameObject.FindGameObjectWithTag ("Rule").GetComponent<LNRule>();
		_rule.UpdatePawnList ();
		move_dungeon (); // update postion

		// find talk object
		_cutscene = GameObject.FindGameObjectWithTag ("Cutscene");
		//

		// set hp bar
		_battery = GetComponentInChildren<LNUIBarCtrl>();
		if(_battery != null) {
			_battery._range = _hp;
			_battery.SetValue( _hp );
		}
		//

		if(transform.FindChild ("Empty") != null) {
			_empty = transform.FindChild ("Empty").gameObject;
			_empty.SetActive(false);
		}
	}

	void Awake () 
	{
		// fixed frame rate
		Application.targetFrameRate = 30;
	}
	
	// Update is called once per frame
	void Update () {
		Update_state ();
		update_emotion ();
		updateShadow ();
	}

	void update_emotion () {
		// look at main camera
		Vector3 v = GameObject.FindGameObjectWithTag("3DCamera").transform.position;
		for(int i = 0; i < _emotions.Length; i++) {
			if(_emotions[i] != null) {
				v.y = _emotions[i].transform.position.y;
				_emotions[i].transform.LookAt( v );
			}
		}

		// battery
		if(_battery != null) {
			v.y = _battery.transform.position.y;
			_battery.transform.LookAt( v );
		}
		if(_empty != null) 
		{
			v.y = _empty.transform.position.y;
			_empty.transform.LookAt( v );
		}
		//
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
				if(v.magnitude < _sak) {
					Emotion(eEMOTION.TALK);
					// enable talk scene
					if(_cutscene) {
						// check kiosk
						if(_npc == eNPC.GATE ) {
							_cutscene.GetComponent<LNCutsceneCtrl>().Enable( _npc_cutscene );
						} else {
							_cutscene.GetComponent<LNCutsceneCtrl>().Enable( _npc_cutscene );
						}
					}

					ChangeState(eSTATE.TALK);
				}
			}
		}
	}

	void state_damage( ) {
		_state_delta += Time.deltaTime;

		// check exit
		if(_state_delta > _stun_delta) {

			// hp == 0
			if(_hp <= 0) {
				// destory self
				Destroy( transform.gameObject );

				return;
			}
			//

			Emotion (eEMOTION.NONE);
			ChangeState( eSTATE.STAY );

			// disable hit effect
			// active hit effect
			Transform hit = transform.FindChild ("EffectHit");
			if(hit) {
				hit.gameObject.SetActive(false);
			}

		}
	}

	// talk state(NPC)
	void state_talk( ) {
		bool bExit = false;

		if(_target) {
			Vector3 v = _target.transform.position - transform.position;
			if(v.magnitude > _sak) {
				bExit = true;
			}
		}
		else {
			bExit = true;
		}

		if(bExit) {
			if(_cutscene) {
				_cutscene.GetComponent<LNCutsceneCtrl>().Disable(); // disable talk scene
			}
			//Emotion(eEMOTION.NONE);
			ChangeState(eSTATE.STAY);
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
		if(_current_state == eSTATE.DAMAGE || _current_state == eSTATE.READY)
			return;

		// triger call back
		if(_corridor) {
			LNDungeonCtrl ctrl = _corridor.GetComponent<LNDungeonCtrl>();
			if(ctrl)
				ctrl.Triger( transform.gameObject, LNDungeonCtrl.TRIGER.DAMAGE );
		}

		// active hit effect
		Transform hit = transform.FindChild ("EffectHit");
		if(hit) {

			// effect direction
			Vector3 dir = source.transform.position;
			dir.y = hit.position.y;

			//Debug.Log("SOURCE : " + dir);

			hit.LookAt(dir);
			//

			hit.gameObject.SetActive(true);
			hit.gameObject.GetComponent<LNEffectCtrl>().Play();
		}

		// knock back
		Vector3 v = transform.position - source.transform.position;
		v.Normalize ();
		v.y = 0.0f;
		v *= 0.25f;
		move (v);
		//

		// Damage process

		// test
		_hp -= 20;
		if(_hp < 0) {
			_hp = 0;
		}
		//

		// Delete action
		if(_hp == 0 && _empty != null) {
			_empty.SetActive(true);
			_battery.transform.gameObject.SetActive(false);
			_avatar.SetActive(false);
		}

		// set hp bar
		if(_battery != null) {
			_battery.SetValue( _hp );
		}
		//

		//

		Emotion (eEMOTION.SMASH);
		ChangeState (eSTATE.DAMAGE);
	}

	// get Talk from source
	public override void Talk(GameObject source) {
		// Check Talk State
		if(_current_state != eSTATE.TALK)
			return;

		// talk event
		if(_npc == eNPC.GATE) {
			// gate
			Debug.Log("GATE");
			Application.LoadLevel(_npc_target);
		} else if(_npc == eNPC.STORE) {
			Debug.Log("store");
			Application.LoadLevel(_npc_target);
		}
	}


	// move
	bool move( Vector3 mov ) {
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
