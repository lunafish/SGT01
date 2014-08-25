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
	public int _base_hp = 100;
	public int _base_mp = 25;
	public int _hp = 100; // hp
	public int _mp = 100; // mp

	// pawn property
	public int _lv = 1; // level
	public int _atk = 10; // attack
	public int _def = 0; // deffence
	public int _luk = 0; // lucky
	public int _str = 0; // strength
	public int _dex = 0; // dexterity
	public int _int = 0; // intelligency
	//

	public eAttack _attackType = eAttack.SMASH;
	public float _shortRangeAttack = 2.0f; // short range attack
	public float _longRangeAttack = 10.0f; // long ragen attack

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

	// tick
	public float _tick = 0.3f; // 1 tick

	// for collidor
	protected BoxCollider _box = null;

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
		MOVE,
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
	public virtual void Damage(GameObject source, int damage) {

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
				} else {
					_corridor = null;
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

	// Action
	protected bool checkBound( Vector3 vec ) {
		if(_box == null) {
			// for collidor
			_box = GetComponentInChildren<BoxCollider>();
		}
		
		// make 4 side
		Vector3 v1 = _box.bounds.min;
		Vector3 v2 = _box.bounds.max;
		Vector3 v3 = _box.bounds.min;
		Vector3 v4 = _box.bounds.max;
		v3.z = v2.z;
		v4.z = v1.z;
		
		Vector3 margin = new Vector3 (0.0f, 1.0f, 0.0f);
		
		RaycastHit hit;
		bool side1 = Physics.Raycast (v1 + vec + margin, -Vector3.up, out hit);
		bool side2 = Physics.Raycast (v2 + vec + margin, -Vector3.up, out hit);
		bool side3 = Physics.Raycast (v3 + vec + margin, -Vector3.up, out hit);
		bool side4 = Physics.Raycast (v4 + vec + margin, -Vector3.up, out hit);
		
		if ((side1 == true) && (side2 == true) && (side3 == true) && (side4 == true)) {
			return true;
		}
		
		return false;
	}


	// get target
	public GameObject GetTarget() { 
		return _target;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	// property
	protected void updateProperty( ) {
		// HP = Base HP + ((LV-1) * STR)
		_hp = GetMaxHP();

		// MP = Base MP + ((LV-1) * INT)
		_mp = GetMaxMP();
	}

	// get MAX HP
	public int GetMaxHP( ) {
		// HP = Base HP + ((LV-1) * STR)
		return (_base_hp + ((_lv - 1) * _str));
	}

	// get MAX MP
	public int GetMaxMP( ) {
		// MP = Base MP + ((LV-1) * INT)
		return (_base_mp + ((_lv - 1) * _int));
	}

	// get attack value
	public int GetATK( ) {
		// ATK = atk + ((lv - 1) * str)
		return (_atk + ((_lv - 1 ) * _str));
	}

	// get deffence value
	public int GetDEF() {
		// DEF = def + ((lv-1) * def)
		return (_def + ((_lv - 1 ) * _def));
	}
}
