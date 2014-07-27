using UnityEngine;
using System.Collections;

public class LNSummoner : MonoBehaviour {
	public SkinnedMeshRenderer _ancker = null; // clane attach point
	private float _anckerMargin = -0.75f;

	public GameObject _corridor; // corridor tile

	protected Animator _anim; // animator
	protected GameObject _shadow;

	protected GameObject _regen; // regen object
	protected bool _isRegen = false; // regen mode?

	// for start trigger
	public float _trigger = 4.0f; // triger length to player
	public float _triggerDelay = 8.0f; // next triger delay
	public string _cargo = "pawn_00"; // regen monster prefebs name
	public int _maxCargo = 1; // max regen cargo
	private float _delayDelta = 0.0f; // delay delta
	private int _cargoCount = 0; // cargo count

	// player
	private GameObject _player;

	// test
	private bool isTest = false;

	// Use this for initialization
	void Start () {
		Init ();
	}
	
	// Update is called once per frame
	void Update () {
		updateAncker (); // update Ancker
		updateShadow (); // update shadow
		updateTrigger (); // update trigger
		/*
		// test
		GameObject player = GameObject.FindGameObjectWithTag ("Player");
		if(player && !isTest) {
			Vector3 v = player.transform.position - transform.position;
			if(v.magnitude < 4.0f) {
				isTest = true;
				Regen("pawn_00");
			}
		}
		//
		*/
	}

	void Init( ) {
		_anim = GetComponentInChildren<Animator> (); // get animator
		_shadow = transform.FindChild ("Shadow").gameObject; // get shadow object
		_cargoCount = 0; // init cargocount
		_delayDelta = _triggerDelay; // init delay delta
		_player = GameObject.FindGameObjectWithTag ("Player"); // get player object
	}

	// set Regen Object
	public void Regen( string name ) {
		Debug.Log ("Regen");
		// make object
		_regen = Instantiate(Resources.Load("prefabs/" + name)) as GameObject;	
		_regen.transform.position = _ancker.bounds.center + new Vector3(0.0f, _anckerMargin, 0.0f);
		_regen.transform.eulerAngles = transform.eulerAngles;
		_regen.GetComponent<LNAIPawn> ()._corridor = _corridor;
		_regen.GetComponent<LNAIPawn> ().ChangeState (LNAIPawn.eSTATE.READY);
		//
		GetComponentInChildren<Animator> ().SetBool ("isMove", true);
		_isRegen = true; // set regen state
	}

	// update Ancker
	void updateAncker( ) {
		if(!_isRegen) {
			return;
		}

		AnimationInfo[] state = _anim.GetCurrentAnimationClipState (0);
		if(state[0].clip.name == "crane_move") {
			AnimatorStateInfo info =  _anim.GetCurrentAnimatorStateInfo (0);
			//Debug.Log(info.normalizedTime);
			if(info.normalizedTime >= 1.0f) {
				_isRegen = false;
				// regen end
				_regen.transform.position = _corridor.transform.position;
				_regen.GetComponent<LNAIPawn> ().ChangeState (LNAIPawn.eSTATE.STAY); // state change
				_regen = null;
				_anim.SetBool ("isMove", false);
				//
				return;
			}
		}

		_regen.transform.position = _ancker.bounds.center + new Vector3(0.0f, _anckerMargin, 0.0f); // move regen object
	}

	// update shadow
	void updateShadow( ) {
		if(_corridor) {
			// set shadow postion on corridor
			_shadow.transform.position = _corridor.transform.position;
		}
	}

	float snap( float fin, float fsnap ) {
		int n = (int)(fin / fsnap);
		float fout = fsnap * (float)n;

		return fout;
	}

	bool triggerPlayer( ) {
		// check delay
		if(_delayDelta < _triggerDelay )
			return false;

		Vector3 dir = _player.transform.position - transform.position;
		dir.y = 0.0f; // ignore y

		// check trigger length
		if( dir.magnitude > _trigger)
			return false;

		// rotate
		dir.Normalize ();
		Quaternion rot = Quaternion.LookRotation( dir );

		// snap rotation
		rot.y = snap (rot.y, 0.5f);
		rot.w = snap (rot.w, 0.5f);
		/*
		if(rot.y >= 0.0f && rot.y < 0.5f)
			rot.y = 0.0f;
		else if(rot.y >= 0.5 && rot.y < 1.0f)
			rot.y = 0.5f;
		else if(rot.y > -0.5f && rot.y < 0.0f)
			rot.y = 0.0f;
		else if(rot.y > -1.0f && rot.y < -0.5f)
			rot.y = -0.5f;

		if(rot.w >= 0.0f && rot.w < 0.5f)
			rot.w = 0.0f;
		else if(rot.w >= 0.5 && rot.w < 1.0f)
			rot.w = 0.5f;
		else if(rot.w > -0.5f && rot.w < 0.0f)
			rot.w = 0.0f;
		else if(rot.w > -1.0f && rot.w < -0.5f)
			rot.w = -0.5f;
		*/	
		//

		//Debug.Log (rot);

		transform.rotation = rot;
		//

		return true;
	}

	// update regen triger
	void updateTrigger( ) {
		_delayDelta += Time.deltaTime;
		// check player
		if(triggerPlayer() && (_cargoCount < _maxCargo)) {
			Regen("pawn_00");
			_cargoCount++;
			_delayDelta = 0.0f;
		}
	}
}
