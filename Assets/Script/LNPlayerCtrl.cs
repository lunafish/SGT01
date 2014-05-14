using UnityEngine;
using System.Collections;

public class LNPlayerCtrl : MonoBehaviour {

	public float _speed = 1.0f;
	public float _rotate_speed = 1.0f;
	public GameObject _avatar;
	public GameObject _camera;

	private Vector3 _mouse_start_pos;
	private Vector3 _mouse_pos;

	private Vector3 _avatar_lookat;
	private Animator _anim;

	struct LNTouch {
		public bool use;
		public int id;
		public Vector3 org_pos;
		public Vector3 pos;
	};

	private LNTouch[] _touches = new LNTouch[5];

	private float _cam_length;
	private Vector3 _cam_target_pos;

	// Use this for initialization
	void Start () {
		_avatar_lookat = transform.position + transform.forward;
		_anim = _avatar.GetComponent<Animator>();

		_touches[0].use = false;
		_touches[1].use = false;
		_touches[2].use = false;
		_touches[3].use = false;
		_touches[4].use = false;

		Vector3 v = transform.position - _camera.transform.position;
		_cam_length = v.magnitude;
		_cam_target_pos = _camera.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		Translate ();	

	}

	void Move(Vector3 v) {
		Vector3 mov = new Vector3 (v.x, 0.0f, v.y);
		mov.Normalize ();
		mov *= (_speed * 0.05f);
		Vector3 vec = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * mov;
		RaycastHit hit;
		Vector3 margin = new Vector3 (0.0f, 0.5f, 0.0f);
		if (Physics.Raycast (transform.position + vec + margin, -Vector3.up, out hit)) {
			transform.Translate(mov); // move
			_avatar_lookat = transform.position + vec;
			_avatar.transform.LookAt( _avatar_lookat );
			Update_camera ();
		}
	}
	
	void Rotate(Vector3 v) {
		if( Mathf.Abs( v.x ) > 10.0f ) {
			float f = _rotate_speed * (v.x * 0.1f);
			_camera.transform.Rotate(0.0f, f, 0.0f);
			Update_camera ();
		}
	}

	bool isMoveTouch( int n ) {
		float f = _touches[n].org_pos.x / Screen.width;
		if (f < 0.5f) {
			return true;
		}
		return false;
	}
	
	// Player translate
	void Translate( ) {
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

							// camera
							if( isMoveTouch( j ) ) {
								transform.rotation = _camera.transform.rotation;
							}

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
							v = _touches[j].pos - _touches[j].org_pos;

							if( isMoveTouch( j ) ) {
								_anim.SetBool ("isRun", true);
								Move ( v );
							} else {
								Rotate ( v );
							}					
							break;
						}
					}

				}
				else if(Input.GetTouch(i).phase == TouchPhase.Ended) {
					for(int j = 0; j < 5; j++) {
						if(_touches[j].use == true && _touches[j].id == Input.GetTouch(i).fingerId ) {
							if( isMoveTouch( j ) ) {
								_anim.SetBool ("isRun", false);
							}

							_touches[j].use = false;
						}
					}
				}
			}
		}

		#else
		if(Input.GetMouseButtonDown(0) == true) {
			_mouse_start_pos = Input.mousePosition;

			float f = _mouse_start_pos.x / Screen.width;
			if(f < 0.5f) {
				transform.rotation = _camera.transform.rotation;
			}
		}
		else if(Input.GetMouseButton(0) == true) {
			_mouse_pos = Input.mousePosition;
			Vector3 v = _mouse_pos - _mouse_start_pos;

			float f = _mouse_start_pos.x / Screen.width;
			if(f < 0.5f) {
				_anim.SetBool ("isRun", true);
				Move ( v );
			} else {
				Rotate ( v );
			}
		}
		else if (Input.GetMouseButtonUp(0) == true) {
			_anim.SetBool ("isRun", false);
		}
		#endif
	}

	// camera update
	void Update_camera() {
		_camera.transform.position = transform.position;
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
		mov *= 0.0625f;
		cam.transform.position = cam.transform.position + mov;
		
	}
}
