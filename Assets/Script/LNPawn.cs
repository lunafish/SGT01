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
	public int _hp = 100;
	public int _mp = 100;
	public int _attack = 100;
	public int _defence = 0;
	public eAttack _attackType = eAttack.SMASH;
	public float _short_attack_range = 2.0f;
	public float _long_attack_range = 10.0f;

	// for pawn grid
	public int _x = -1, _y = -1;

	// for battle
	protected GameObject _target = null; // target object

	// for state machine
	protected eSTATE _current_state = eSTATE.STAY; // current state
	protected eSTATE _backup_state; // state backup
	protected float _state_delta = 0.0f;

	// pawn
	public enum ePawn {
		PLAYER = 0,
		NPC,
		ENEMY,
	};

	// state machine
	public enum eSTATE {
		STAY = 0,
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
	protected void change_state( eSTATE state ) {
		_backup_state = _current_state; // state backup
		_current_state = state; // change state
		_state_delta = 0.0f; // init state delta;
	}

	// move dungeon grid
	protected void move_dungeon( ) {
		// update postion
		RaycastHit hit;
		if(Physics.Raycast (transform.position, -Vector3.up, out hit) == true) {
			GameObject obj = null;
			if(hit.transform.parent) {
				obj = hit.transform.parent.gameObject;
			}

			if(obj) {
				LNDungeonCtrl ctrl = obj.GetComponent<LNDungeonCtrl>();
				if(ctrl) {
					ctrl.AddPawn( transform.gameObject );
				}
			}
		}
		//
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
}
