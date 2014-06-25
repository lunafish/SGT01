using UnityEngine;
using System.Collections;

public class LNPlayerCtrl : LNPawn {
	// public
	public GameObject _camera; // camera object
	public bool _viewer = false;
	public float _range_attack = 2.0f;

	// for mouse
	private Vector3 _mouse_start_pos;
	private Vector3 _mouse_pos;

	// for touch
	struct LNTouch {
		public bool use;
		public int id;
		public Vector3 org_pos;
		public Vector3 pos;
	};

	// const value
	private const int MAX_INPUT = 5;
	private const int TOUCH_DOWN = 0;
	private const int TOUCH_HOLD = 1;
	private const int TOUCH_UP = 2;

	private LNTouch[] _touches = new LNTouch[MAX_INPUT];

	// for camera
	private float _cam_length;
	private Vector3 _cam_target_pos;
	private Vector3 _avatar_lookat;

	// playter animation
	private Animator _anim;

	// input
	struct LNInput {
		public bool use;
		public bool rotate;
		public int state;
		public Vector3 org_pos;
		public Vector3 pos;
		public float move_delta;
	};
	private LNInput[] _inputs = new LNInput[MAX_INPUT];

	// for attack
	float _attack_delay = 1.0f;

	// for dash
	float _dash_delay = 1.0f; // _dash time
	float _dash_speed = 4.0f; // _speed * _dash_speed
	Vector3 _dash_dir;

	// for animation
	enum eANI { 
		STAY = 0,
		ATTACK_STAY,
		RUN,
		ATTACK_GUN,
		ATTACK_BLADE,
		ATTACK_DASH,
	};

	// Use this for initialization
	void Start () {
		// set avatar look
		_avatar_lookat = transform.position + transform.forward;
		_anim = _avatar.GetComponent<Animator>(); // get avatar animation controler

		// init value
		for(int i=0; i < MAX_INPUT; i++) {
			_touches[i].use = false;
			_inputs[i].use = false;
		}

		// set camera at avatar pos
		Vector3 v = transform.position - _camera.transform.position;
		_cam_length = v.magnitude;
		_cam_target_pos = _camera.transform.position;

		_camera.transform.position = transform.position;
		_camera.transform.rotation = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {
		UpdateInput();	
		UpdatePlayer();
	}

	// Action

	// move
	void Move(Vector3 v) {
		Vector3 mov = new Vector3 (v.x, 0.0f, v.y);
		mov.Normalize ();
		mov *= (_speed * 0.05f);
		Vector3 vec = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * mov;
		RaycastHit hit;
		Vector3 margin = new Vector3 (0.0f, 0.5f, 0.0f);

		BoxCollider box = GetComponent<BoxCollider> ();
		bool bMax = Physics.Raycast (box.bounds.max + vec + margin, -Vector3.up, out hit);
		bool bMin = Physics.Raycast (box.bounds.min + vec + margin, -Vector3.up, out hit);

		if ((bMax == true) && (bMin == true)) {
			transform.Translate(mov); // move
			_avatar_lookat = transform.position + vec;
			_avatar.transform.LookAt( _avatar_lookat );

			// move dungeon
			move_dungeon( );

			if(!_viewer)
				Update_camera ();
		}

		Update_target ();
	}

	// dash
	bool Move_dash(Vector3 v) {
		Vector3 mov = new Vector3 (v.x, 0.0f, v.z);
		mov *= (_speed * 0.05f);
		RaycastHit hit;
		Vector3 margin = new Vector3 (0.0f, 0.5f, 0.0f);
		
		BoxCollider box = GetComponent<BoxCollider> ();
		bool bMax = Physics.Raycast (box.bounds.max + mov + margin, -Vector3.up, out hit);
		bool bMin = Physics.Raycast (box.bounds.min + mov + margin, -Vector3.up, out hit);
		
		if ((bMax == true) && (bMin == true)) {
			//transform.Translate(mov); // move
			transform.position += mov;
			_avatar_lookat = transform.position + mov;
			_avatar.transform.LookAt( _avatar_lookat );
			if(!_viewer)
				Update_camera ();
		} else {
			return false;
		}

		return true;
	}

	// rotate
	bool Rotate(Vector3 v) {
		// check rotate bound
		if( Mathf.Abs( v.x ) > 16.0f && Mathf.Abs (v.y) < 64.0f) {
			float f = _rotate_speed * (v.x * 0.1f);
			_camera.transform.Rotate(0.0f, f, 0.0f);
			if(!_viewer)
				Update_camera ();
			return true;
		}
		return false;
	}

	// Action
	void Action( ) {
		if(_target.GetComponent<LNPawn>()._type == ePawn.NPC) {
			// npc action
			// get rule book and action target
			GameObject rule = GameObject.FindGameObjectWithTag ("Rule");
			_target = rule.GetComponent<LNRule> ().FindTarget ( transform.gameObject );
			rule.GetComponent<LNRule> ().Action( transform.gameObject, _target );
			//
		} else if(_target.GetComponent<LNPawn>()._type == ePawn.ENEMY) {
			change_state( eSTATE.ATTACK );
			Attack();
		}
	}
	
	// attack enemy
	void Attack( ) {
		_attack_delay = 0.5f; // set attack delay

		// process attack
		// get attack type
		Vector3 v = _target.transform.position - transform.position;
		if(v.magnitude < _range_attack) {
			// range attack
			_attackType = eAttack.SMASH;
			ChangeAni (eANI.ATTACK_BLADE); // change attack ani
		} else {
			// long range attack
			_attackType = eAttack.RANGE;
			ChangeAni (eANI.ATTACK_GUN); // change attack ani
		}

		// rotate player
		v.Normalize ();
		_avatar_lookat = transform.position + v;
		_avatar_lookat.y = transform.position.y; // lock y axis
		_avatar.transform.LookAt (_avatar_lookat);

		// get rule book and attack target
		GameObject rule = GameObject.FindGameObjectWithTag ("Rule");
		_target = rule.GetComponent<LNRule> ().FindTarget ( transform.gameObject );
		rule.GetComponent<LNRule> ().Attack( transform.gameObject, _target );
		//

	}

	// dash
	void Dash( ) {
		if(_target == null) {
			Debug.Log("DASH ERROR : No Target");
			return;
		}

		_speed *= _dash_speed; // set dash speed

		// set dash direction
		_dash_dir = _target.transform.position - transform.position;
		_dash_dir.Normalize ();

		ChangeAni (eANI.ATTACK_DASH); // dash ani
		change_state(eSTATE.DASH); // change state
	}

	// change player animation
	void ChangeAni( eANI ani ) {
		if(ani == eANI.STAY ) {
			_anim.SetBool ("isRun", false);
			_anim.SetBool ("isAttackBlade", false);
			_anim.SetBool ("isAttackGun", false);
			_anim.SetBool ("isAttackStay", false);
			_anim.SetBool ("isAttackDash", false);
		} else if(ani == eANI.RUN ) {
			_anim.SetBool ("isRun", true);
			_anim.SetBool ("isAttackBlade", false);
			_anim.SetBool ("isAttackGun", false);
			_anim.SetBool ("isAttackStay", false);
			_anim.SetBool ("isAttackDash", false);
		} else if(ani == eANI.ATTACK_STAY ) {
			_anim.SetBool ("isRun", false);
			_anim.SetBool ("isAttackBlade", false);
			_anim.SetBool ("isAttackGun", false);
			_anim.SetBool ("isAttackStay", true);
			_anim.SetBool ("isAttackDash", false);
		} else if(ani == eANI.ATTACK_BLADE ) {
			_anim.SetBool ("isRun", false);
			_anim.SetBool ("isAttackBlade", true);
			_anim.SetBool ("isAttackGun", false);
			_anim.SetBool ("isAttackStay", true);
			_anim.SetBool ("isAttackDash", false);
		} else if(ani == eANI.ATTACK_GUN ) {
			_anim.SetBool ("isRun", false);
			_anim.SetBool ("isAttackBlade", false);
			_anim.SetBool ("isAttackGun", true);
			_anim.SetBool ("isAttackStay", true);
			_anim.SetBool ("isAttackDash", false);
		} else if(ani == eANI.ATTACK_DASH ) {
			_anim.SetBool ("isRun", false);
			_anim.SetBool ("isAttackBlade", false);
			_anim.SetBool ("isAttackGun", false);
			_anim.SetBool ("isAttackStay", true);
			_anim.SetBool ("isAttackDash", true);
		}
	}

	bool isMoveTouch( int n ) {
		float f = _inputs[n].org_pos.x / Screen.width;
		if (f < 0.5f) {
			return true;
		}
		return false;
	}

	// Player translate
	void UpdateInput( ) {
		// init input value
		for(int i=0; i < MAX_INPUT; i++) {
			_inputs[i].use = false;
		}

		#if UNITY_IPHONE
		if (Input.touchCount > 0 ) {
			for(int i = 0; i < Input.touchCount; i++) {
				if(Input.GetTouch(i).phase == TouchPhase.Began) {
					//_touch_start_pos[i] = Input.GetTouch(i).position;

					for(int j = 0; j < 5; j++) {
						if(_touches[j].use == false) {
							_touches[j].use = true;
							_touches[j].id = Input.GetTouch(i).fingerId;
							_touches[j].org_pos = Input.GetTouch(i).position;

							_inputs[j].use = true;
							_inputs[j].rotate = false;
							_inputs[j].state = TOUCH_DOWN;
							_inputs[j].org_pos = Input.GetTouch(i).position;
							_inputs[j].move_delta = 0.0f;
							break;
						}
					}
				}
				else if(Input.GetTouch(i).phase == TouchPhase.Moved || Input.GetTouch(i).phase == TouchPhase.Stationary) {
					//_touch_pos[i] = Input.GetTouch(i).position;
					//Vector3 v = _touch_pos[i] - _touch_start_pos[i];
					Vector3 v;
					for(int j = 0; j < 5; j++) {
						if(_touches[j].use == true && _touches[j].id == Input.GetTouch(i).fingerId ) {
							_touches[j].pos = Input.GetTouch(i).position;

							_inputs[j].use = true;
							_inputs[j].state = TOUCH_HOLD;
							_inputs[j].pos = Input.GetTouch(i).position;
							break;
						}
					}

				}
				else if(Input.GetTouch(i).phase == TouchPhase.Ended) {
					for(int j = 0; j < 5; j++) {
						if(_touches[j].use == true && _touches[j].id == Input.GetTouch(i).fingerId ) {

							_inputs[j].use = true;
							_inputs[j].state = TOUCH_UP;
							_inputs[j].pos = Input.GetTouch(i).position;
							_touches[j].use = false;
						}
					}
				}
			}
		}

		#else
		if(Input.GetMouseButtonDown(0) == true) {
			_mouse_start_pos = Input.mousePosition;

			_inputs[0].use = true;
			_inputs[0].rotate = false;
			_inputs[0].state = TOUCH_DOWN;
			_inputs[0].org_pos = Input.mousePosition;
			_inputs[0].move_delta = 0.0f;
		}
		else if(Input.GetMouseButton(0) == true) {
			_inputs[0].use = true;
			_inputs[0].state = TOUCH_HOLD;
			_inputs[0].pos = Input.mousePosition;
		}
		else if (Input.GetMouseButtonUp(0) == true) {
			_inputs[0].use = true;
			_inputs[0].state = TOUCH_UP;
			_inputs[0].pos = Input.mousePosition;
		}
		#endif
	}

	// state machine
	void state_move( ) {
		for(int i = 0; i < MAX_INPUT; i++) {
			if(_inputs[i].use == true) {
				if(_inputs[i].state == TOUCH_DOWN) {
					if(isMoveTouch(i)) {
						transform.rotation = _camera.transform.rotation;
					}
				}
				else if(_inputs[i].state == TOUCH_HOLD) {
					Vector3 v = _inputs[i].pos - _inputs[i].org_pos;
					_inputs[i].move_delta += v.magnitude;

					if( isMoveTouch( i ) ) {
						_anim.SetBool ("isRun", true);
						Move ( v );
					} else {
						_inputs[i].rotate = Rotate ( v ); // rotate check
					}					
				}
				else if(_inputs[i].state == TOUCH_UP) {
					_anim.SetBool ("isRun", false);
					if( isMoveTouch( i ) == false ) {
						if(_inputs[i].move_delta < 10.0f) {
							if(_target) {
								Action();
							}
						} else {
							if(_inputs[i].rotate == false) {
								// process dash
								Vector3 v = _inputs[i].pos - _inputs[i].org_pos;
								if( (Mathf.Abs(v.x) < 40.0f) && v.y > 10.0f ) {
									Debug.Log("Dash");
									Dash();
								}
							}
						}
					}
				}
			}
		}
	}

	// dash state
	void state_dash( ) {
		_state_delta += Time.deltaTime;

		// dash move
		bool state = Move_dash (_dash_dir);

		// Approch check
		Vector3 v = _target.transform.position - transform.position;
		if (v.magnitude < 2.0f) {
			state = false;
		}

		if ( (_state_delta > _dash_delay) || (state == false)) {
			_speed /= _dash_speed;
			ChangeAni (eANI.ATTACK_STAY);
			change_state( eSTATE.STAY );
		}
	}

	// attack state
	void state_attack( ) {
		_state_delta += Time.deltaTime;

		if (_state_delta > _attack_delay) {
			ChangeAni (eANI.ATTACK_STAY);
			change_state( eSTATE.STAY );
		}
	}

	// player update
	void UpdatePlayer( ) {
		// state machine
		if(_current_state == eSTATE.STAY) {
			state_move( );
		} else if(_current_state == eSTATE.DASH) {
			state_dash( );
		} else if(_current_state == eSTATE.ATTACK) {
			state_attack( );
		}
	}

	// camera update
	void Update_camera() {
		_camera.transform.position = transform.position;
		Camera cam = _camera.GetComponentInChildren<Camera> ();
		BoxCollider box = cam.GetComponent<BoxCollider> (); // get box collider
		Vector3 dir = -cam.transform.forward;

		dir *= 0.5f;
		Vector3 pos = _camera.transform.position + dir;
		pos.y = cam.transform.position.y;
		RaycastHit hit;
		for(int i = 0; i < 5; i++) {
			bool bMax = Physics.Raycast (box.bounds.max, -Vector3.up, out hit);
			bool bMin = Physics.Raycast (box.bounds.min, -Vector3.up, out hit);
			if( (bMax == true) && (bMin == true)) {
				Debug.DrawRay(pos, -Vector3.up, Color.green);
			} else {
				Debug.DrawRay(pos, -Vector3.up, Color.red);
				break;
			}

			pos += dir;
		}
		pos.y = cam.transform.position.y;
		//cam.transform.position = pos;

		Vector3 mov = pos - cam.transform.position;
		mov.y = 0.0f;
		mov *= 0.03125f;
		cam.transform.position = cam.transform.position + mov;
		
	}

	// target update
	void Update_target( ) {
		GameObject rule = GameObject.FindGameObjectWithTag ("Rule");
		_target = rule.GetComponent<LNRule> ().FindTarget ( transform.gameObject );
	}

	// state function
	public override void Damage(GameObject source) {
		
	}
}
