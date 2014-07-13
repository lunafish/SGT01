//
// For dynamic object (player, npc, monster...)
//

using UnityEngine;
using System.Collections;

public class LNPawn : MonoBehaviour {
	public float _speed = 1.0f; // default player move speed
	public float _rotate_speed = 1.0f; // default camera rotate speed
	public GameObject _avatar; // avatar object
	public ePawn _type; // pawn type
	public float _sight_length = 10.0f; // sight length

	// pawn default state
	public int _hp = 100; // hp
	public int _mp = 100; // mp
	public int _sp = 0; // sp
	public int _crt = 0; // critical

	// pawn gene
	public int _str = 0; // strength
	public int _int = 0; // intelligence
	public int _wis = 0; // wisdom
	public int _dex = 0; // dexterity
	public int _con = 0; // constitution

	public eAttack _attackType = eAttack.SMASH;
	public float _sak = 2.0f; // short range attack
	public float _lak = 10.0f; // long ragen attack

	// for pawn grid
	public int _x = -1, _y = -1;

	// for battle
	protected GameObject _target = null; // target object

	// for state machine
	protected eSTATE _current_state = eSTATE.STAY; // current state
	protected eSTATE _backup_state; // state backup
	protected float _state_delta = 0.0f;

	// for dungeon
	public GameObject _corridor = null; // _corridor tile

	// shadow
	protected GameObject _shadow = null;

	// pawn
	public enum ePawn {
		PLAYER = 0,
		NPC,
		ENEMY,
	};

	// state machine
	public enum eSTATE {
		READY = 0,
		STAY,
		DASH,
		ATTACK,
		DAMAGE,
		DIE,
		TALK,
	};

	// attack
	public enum eAttack {
		SMASH = 0,
		RANGE,
		SPECIAL,
	};

	// get target range
	public float Range(GameObject target) {
		Vector3 v = transform.position - target.transform.position;
		return v.magnitude;
	}

	// Move Pawn
	public void Move( Vector3 pos ) {
		transform.position = pos;
		move_dungeon ();
	}

	// set target
	public virtual void Target(GameObject target) {
		_target = target;
	}

	// get damage from enemy
	public virtual void Damage(GameObject source) {

	}

	// get Talk from source
	public virtual void Talk(GameObject source) {

	}

	// change pawn state
	public void ChangeState( eSTATE state ) {
		_backup_state = _current_state; // state backup
		_current_state = state; // change state
		_state_delta = 0.0f; // init state delta;
	}

	// move dungeon grid
	protected void move_dungeon( ) {
		// update postion
		_corridor = null;
		RaycastHit hit;
		if(Physics.Raycast (transform.position + new Vector3(0.0f, 0.5f, 0.0f), -Vector3.up, out hit) == true) {
			if(hit.transform.parent) {
				_corridor = hit.transform.parent.gameObject;
			}

			if(_corridor) {
				LNDungeonCtrl ctrl = _corridor.GetComponent<LNDungeonCtrl>();
				if(ctrl) {
					ctrl.AddPawn( transform.gameObject );
				}
			}
		}
		//
	}

	protected void updateShadow( ) {
		if(_shadow == null) {
			_shadow = transform.FindChild ("Shadow").gameObject; // get shadow object
		}

		if(_corridor && _shadow) {
			// only dungeon
			if(_corridor.GetComponent<LNDungeonCtrl>()) {
				// set shadow postion on corridor
				_shadow.transform.position = _corridor.transform.position;
			}
		}
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}
	
}
