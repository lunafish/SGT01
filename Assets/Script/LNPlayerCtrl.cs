using UnityEngine;
using System.Collections;

public class LNPlayerCtrl : LNPawn {
	// public
	public GameObject _camera; // camera object
	public bool _viewer = false;
	public float _range_attack = 2.0f;

	// for camera
	private float _cam_length;
	private Vector3 _cam_target_pos;
	private Vector3 _avatar_lookat;

	// playter animation
	private Animator _anim;

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

	// for sound
	public AudioSource _sndCrash = null;

	// for pad
	private LNPad _pad = null;

	// Use this for initialization
	void Start () {
		// return point
		Vector3 pos;
		Quaternion rot;
		if(LNUtil.Instance().GetReturnPoint( out pos, out rot ) ) {
			Debug.Log("Return Point!");
			transform.position = pos;
			transform.rotation = rot;
		}
		//

		// set avatar look
		_avatar_lookat = transform.position + transform.forward;
		_anim = _avatar.GetComponent<Animator>(); // get avatar animation controler

		// find pad
		_pad = GameObject.FindGameObjectWithTag("Input").GetComponent<LNPad>();

		// set camera at avatar pos
		Vector3 v = transform.position - _camera.transform.position;
		_cam_length = v.magnitude;
		_cam_target_pos = _camera.transform.position;

		_camera.GetComponent<LNCameraCtrl>().SetPostion( transform.position );
		_camera.GetComponent<LNCameraCtrl>().SetRotation( transform.rotation );

		// default state
		ChangeState (eSTATE.STAY);

		// init preperty
		updateProperty();
	}
	
	// Update is called once per frame
	void Update () {
		//UpdateInput();	
		UpdatePlayer();

	}


	// move
	void move(Vector3 v) {
		// check cutscene
		if(LNCutsceneCtrl._isEnable) {
			return;
		}

		Vector3 mov = new Vector3 (v.x, 0.0f, v.y);
		mov.Normalize ();
		mov *= (_speed * Time.deltaTime);
		Vector3 vec = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * mov;

		// crash enemy
		bool bCrash = false;
		if(_corridor != null) {
			if(_corridor.GetComponent<LNDungeonCtrl>().Crash( gameObject, vec )) {
				bCrash = true;
			}
		}

		if(checkBound( vec ) && (bCrash == false)) {
			transform.position += vec;
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
	bool move_dash(Vector3 v) {
		Vector3 mov = new Vector3 (v.x, 0.0f, v.z);
		mov *= (_speed * Time.deltaTime);
		RaycastHit hit;
		Vector3 margin = new Vector3 (0.0f, 0.5f, 0.0f);
		
		bool bMax = Physics.Raycast (_box.bounds.max + mov + margin, -Vector3.up, out hit);
		bool bMin = Physics.Raycast (_box.bounds.min + mov + margin, -Vector3.up, out hit);
		
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
		if( Mathf.Abs( v.x ) > 16.0f && Mathf.Abs (v.y) < 128.0f) {
			float f = _rotate_speed * (v.x * 0.1f);
			_camera.GetComponent<LNCameraCtrl>().Rotate(0.0f, f, 0.0f);

			if(!_viewer)
				Update_camera ();
			return true;
		}
		return false;
	}

	// Action
	public void Action() {
		Debug.Log(_target);

		if(_target.GetComponent<LNPawn>()._type == ePawn.NPC) {
			// npc action
			// get rule book and action target
			GameObject rule = GameObject.FindGameObjectWithTag ("Rule");
			_target = rule.GetComponent<LNRule> ().FindTarget ( transform.gameObject );
			rule.GetComponent<LNRule> ().Action( transform.gameObject, _target );
			//
		} else if(_target.GetComponent<LNPawn>()._type == ePawn.ENEMY) {
			ChangeState( eSTATE.ATTACK );
			Attack();
		} 
	}
	
	// attack enemy
	void Attack( ) {
		_attack_delay = 0.5f; // set attack delay

		// process attack
		// get attack type
		Vector3 v = _target.transform.position - transform.position;
		float len = v.magnitude;

		// rotate player
		v.Normalize ();
		_avatar_lookat = transform.position + v;
		_avatar_lookat.y = transform.position.y; // lock y axis
		_avatar.transform.LookAt (_avatar_lookat);

		if(len < _range_attack) {
			// range attack
			_attackType = eAttack.SMASH;
			ChangeAni (eANI.ATTACK_BLADE); // change attack ani
			_camera.GetComponent<LNCameraCtrl>().Effect( LNCameraCtrl.EFFECT.PUSH, _avatar.transform.forward ); // push forawrd
		} else {
			// long range attack
			_attackType = eAttack.RANGE;
			ChangeAni (eANI.ATTACK_GUN); // change attack ani
			_camera.GetComponent<LNCameraCtrl>().Effect( LNCameraCtrl.EFFECT.PUSH, _avatar.transform.forward * -1 ); // push backward

			// bullet attack effect
			// active hit effect
			Transform hit = transform.Find ("EffectBullet");
			if(hit) {
				hit.gameObject.SetActive(true);
				hit.gameObject.transform.rotation = _avatar.transform.rotation;
				hit.gameObject.GetComponent<LNEffectCtrl>().Play();
			}

			// test
			if(_mp > 0) {
				_mp -= 5;
			}
			//
		}

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
		ChangeState(eSTATE.DASH); // change state
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

	// UI Check
	bool checkUI( int touchIndex ) {
		// check ui
		Ray ray = Camera.main.ScreenPointToRay( _pad.getInput(touchIndex).pos); // get mouse ray
		RaycastHit hit; // hit object
		if(Physics.Raycast (ray, out hit, Mathf.Infinity))
		{
			GameObject obj = hit.transform.gameObject;
			if(obj.GetComponent<tk2dSprite>()) {
				Debug.Log("hit : " + obj.name);
				return true; // ui
			}
		}
		//

		return false;
	}

	// state macFhine
	void state_move( ) {
		for(int i = 0; i < LNPad.MAX_INPUT; i++) {
			if(_pad.getInput(i).state == LNPad.TOUCH_DOWN) {
				if(_pad.isMoveTouch(i)) {
					transform.rotation = _camera.transform.rotation;
				}
			}
			else if(_pad.getInput(i).state == LNPad.TOUCH_HOLD) {
				Vector3 v = _pad.getInput(i).pos - _pad.getInput(i).org_pos;

				_pad.SetMoveDelta(i, _pad.getInput(i).move_delta + v.magnitude);
				if( _pad.isMoveTouch( i ) ) {
					_anim.SetBool ("isRun", true);
					move ( v );
				} else {
					_pad.SetRotate (i, Rotate (v)); // rotate check
				}					
			}
			else if(_pad.getInput(i).state == LNPad.TOUCH_UP) {
				_anim.SetBool ("isRun", false);
				//Debug.Log("TOUCH UP");
				if( _pad.isMoveTouch( i ) == false ) {
					if(_pad.getInput(i).move_delta < 10.0f) {


						if(_target != null) {
							Debug.Log("Action : " + _target);
							if(!checkUI(i)) {
								Action();
							}
						} else {
							// default attack motion
							ChangeState( eSTATE.ATTACK );
							_attack_delay = _tick * 2.0f; // set attack delay
							ChangeAni (eANI.ATTACK_BLADE); // change attack ani
							//
						}
					} else {
						if(_pad.getInput(i).rotate == false) {
							// process dash
							Vector3 v = _pad.getInput(i).pos - _pad.getInput(i).org_pos;
							if( (Mathf.Abs(v.x) < 40.0f) && v.y > 10.0f ) {
								Debug.Log("Dash");
								Dash();
							}
						}
					}
				}
				else {
					//Debug.Log("Move");
				}
			}
		}
	}

	// dash state
	void state_dash( ) {
		_state_delta += Time.deltaTime;

		// dash move
		bool state = move_dash (_dash_dir);

		// Approch check
		Vector3 v = _target.transform.position - transform.position;
		if (v.magnitude < 2.0f) {
			state = false;
		}

		if ( (_state_delta > _dash_delay) || (state == false)) {
			_speed /= _dash_speed;
			ChangeAni (eANI.ATTACK_STAY);
			ChangeState( eSTATE.STAY );
		}
	}

	// attack state
	void state_attack( ) {
		_state_delta += Time.deltaTime;

		if (_state_delta > _attack_delay) {
			ChangeAni (eANI.ATTACK_STAY);
			ChangeState( eSTATE.STAY );

			// Effect off
			// active hit effect
			Transform hit = transform.Find ("EffectBullet");
			if(hit) {
				hit.gameObject.SetActive(false);
			}
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
		_camera.GetComponent<LNCameraCtrl>().SetPostion(transform.position);
		Camera cam = _camera.GetComponentInChildren<Camera> ();
		Vector3 dir = -cam.transform.forward;

		dir *= 0.5f;
		Vector3 pos = _camera.transform.position + dir;
		pos.y = cam.transform.position.y;
		RaycastHit hit;
		for(int i = 0; i < 5; i++) {
			if(Physics.Raycast (pos, -Vector3.up, out hit)) {
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
	public override void Damage(GameObject source, int damage) {
		_sndCrash.Play();
		_hp -= damage;
	
		_camera.GetComponent<LNCameraCtrl>().Effect( LNCameraCtrl.EFFECT.SHAKE );

		// test
		if(_hp < 0) {
			_hp = GetMaxHP();
		}
	}
}
