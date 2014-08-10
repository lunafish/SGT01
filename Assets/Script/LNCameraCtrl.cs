/** Camera Controller lunafish 2014-08-10 **/
using UnityEngine;
using System.Collections;

public class LNCameraCtrl : MonoBehaviour {
	// effect type
	public enum EFFECT {
		SHAKE = 0,
		PUSH,
	};

	// check effect
	private EFFECT _effect;
	private bool _bEffect = false;
	private float _delta = 0.0f;
	private float _endTick = 0.0f;
	private Vector3 _orgPostion; // org postion
	private Vector3 _orgLocalPostion; // org postion
	private Vector3 _direction; // direction

	private float _tick = 0.3f; // tick

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(_bEffect) {
			if(_effect == EFFECT.SHAKE) {
				shake( );
			} else if(_effect == EFFECT.PUSH) {
				push( );
			}
		}
	}

	// set effect
	public void Effect( EFFECT type, Vector3 dir = new Vector3()) {
		_effect = type;
		_bEffect = true;
		_delta = 0.0f;
		_endTick = _tick;
		_orgPostion = transform.position;
		_orgLocalPostion = transform.localPosition;
		_direction = dir;

		if(type == EFFECT.PUSH) {
			// default setting
			transform.localPosition += _direction * (_tick * 0.33f);
		}

	}

	// Set camera postion
	public void SetPostion( Vector3 pos ) {
		if(_bEffect) {
			return;
		}
		transform.position = pos;
	}

	// Set cmear rotation
	public void SetRotation( Quaternion rot ) {
		if(_bEffect) {
			return;
		}
		transform.rotation = rot;
	}

	// Rotate
	public void Rotate( float x, float y, float z ) {
		if(_bEffect) {
			return;
		}
		transform.Rotate( x, y, z);
	}

	// check endstate
	bool endEffect( ) {
		_delta += Time.deltaTime;
		if(_delta > _endTick) {
			transform.position = _orgPostion;
			_bEffect = false;
			return true;
		}

		return false;
	}

	// shake effect
	void shake( ) {
		if(endEffect()) {
			return;
		}

		float x = Random.Range(-0.05f, 0.05f);
		float y = Random.Range(-0.05f, 0.05f);
		transform.position = _orgPostion + new Vector3(x, y, 0.0f);
	}

	// push effect
	void push( ) {
		if(endEffect()) {
			return;
		}

		//transform.localPosition += _direction * (_tick * 0.1f);
	}
}
