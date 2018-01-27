using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// input struct
public struct LNInput {
	public bool rotate;
	public int state;
	public Vector3 org_pos;
	public Vector3 pos;
	public float move_delta;
};

public class LNPad : MonoBehaviour {
	public GameObject _press = null;
	public GameObject _move = null;
	public LineRenderer _line = null;

	public GameObject _cam = null;
	public GameObject _camMove = null;
	public LineRenderer _camLine = null;

	public GameObject _dashR = null;
	public GameObject _dashL = null;

	// for touch
	struct LNTouch {
		public bool use;
		public int id;
		public Vector3 org_pos;
		public Vector3 pos;
	};

	// const value
	public const int MAX_INPUT = 5;
	public const int TOUCH_NONE = 0;
	public const int TOUCH_DOWN = 1;
	public const int TOUCH_HOLD = 2;
	public const int TOUCH_UP = 3;

	private LNTouch[] _touches = new LNTouch[MAX_INPUT];

	private LNInput[] _inputs = new LNInput[MAX_INPUT];

	// Use this for initialization
	void Start () {
		_press.gameObject.SetActive (false);
		_move.gameObject.SetActive (false);
		_line.gameObject.SetActive (false);

		_cam.gameObject.SetActive (false);
		_camMove.gameObject.SetActive (false);
		_camLine.gameObject.SetActive (false);

		// init value
		for(int i=0; i < MAX_INPUT; i++) {
			_touches[i].use = false;
			_inputs[i].state = TOUCH_NONE;
		}
	}
	
	// Update is called once per frame
	void Update () {
		UpdateInput ();
		DrawPress ();
	}

	// Player translate
	void UpdateInput( ) {
		// init input value
		for(int i=0; i < MAX_INPUT; i++) {
			_inputs[i].state = TOUCH_NONE;
		}

		int n = Input.touchCount;

		if (n == 0) {
			// for mouse
			n = 1;
		}

		for (int i = 0; i < n; i++) {
			if (touchBegin (i))
				continue;
			if (touchMove (i))
				continue;
			if (touchEnd (i))
				continue;
		}

		n = 0;
		for (int i = 0; i < MAX_INPUT; i++) {
			if (_touches [i].use)
				n++;
		}

		//Debug.Log (n + " " + Input.touchCount);
	}

	public LNInput getInput(int idx = 0) {
		if (idx < MAX_INPUT)
			return _inputs [idx];
		return _inputs [0];
	}

	public void SetMoveDelta(int idx, float value) {
		_inputs [idx].move_delta = value;
	}

	public void SetRotate(int idx, bool value) {
		_inputs [idx].rotate = value;
	}

	void DrawPress()
	{
		GameObject btn = null;
		GameObject mov = null;
		LineRenderer line = null;
		Vector3 pos;

		for (int i = 0; i < MAX_INPUT; i++) {
			if (getInput (i).state == TOUCH_DOWN) {
				if (isMoveTouch (i)) {
					btn = _press;
				} else {
					btn = _cam;
				}

				btn.gameObject.SetActive (true);
				pos = Camera.main.ScreenToWorldPoint (getInput (i).pos);
				pos.z = btn.transform.position.z;
				btn.transform.position = pos;
			} else if(getInput (i).state == TOUCH_HOLD) {
				if (isMoveTouch (i)) {
					btn = _press;
					mov = _move;
					line = _line;
				} else {
					btn = _cam;
					mov = _camMove;
					line = _camLine;
				}
				btn.gameObject.SetActive (true);
				mov.gameObject.SetActive (true);
				line.gameObject.SetActive (true);

				pos = Camera.main.ScreenToWorldPoint (getInput (i).org_pos);
				pos.z = btn.transform.position.z;
				btn.transform.position = pos;

				pos = Camera.main.ScreenToWorldPoint (getInput (i).pos);
				pos.z = mov.transform.position.z;
				mov.transform.position = pos;

				line.SetPosition(0, btn.transform.localPosition);
				line.SetPosition(1, mov.transform.localPosition);
			} else if(getInput (i).state == TOUCH_UP) {
				if (isMoveTouch (i)) {
					btn = _press;
					mov = _move;
					line = _line;
				} else {
					btn = _cam;
					mov = _camMove;
					line = _camLine;
				}
				btn.gameObject.SetActive (false);
				mov.gameObject.SetActive (false);
				line.gameObject.SetActive (false);
			}
		}
	}

	public bool isMoveTouch( int n ) {
		float f = _inputs[n].org_pos.x / Screen.width;
		if (f < 0.5f) {
			return true;
		}
		return false;
	}

	bool touchBegin(int id) {
		// touch
		if (Input.touchCount > id) {
			if (Input.GetTouch (id).phase == TouchPhase.Began) {
				for (int i = 0; i < MAX_INPUT; i++) {
					if (_touches [i].use == false) {
						_touches [i].use = true;
						_touches [i].id = Input.GetTouch (id).fingerId;
						_touches [i].org_pos = Input.GetTouch (id).position;

						_inputs [i].rotate = false;
						_inputs [i].state = TOUCH_DOWN;
						_inputs [i].org_pos = Input.GetTouch (id).position;
						_inputs [i].move_delta = 0.0f;

						return true;
					} else {
						if (_touches [i].id == Input.GetTouch (id).fingerId)
							return true;
					}
				}
			}
		} else {
			// mouse
			if (Input.GetMouseButtonDown (0) == true) {
				_inputs[0].rotate = false;
				_inputs[0].state = TOUCH_DOWN;
				_inputs[0].org_pos = Input.mousePosition;
				_inputs[0].move_delta = 0.0f;
				return true;
			}
			// keyboard
		}

		return false;
	}

	bool touchMove(int id) {

		if (Input.touchCount > id) {
			if (Input.GetTouch (id).phase == TouchPhase.Moved || Input.GetTouch (id).phase == TouchPhase.Stationary) {
				for (int i = 0; i < MAX_INPUT; i++) {
					if (_touches [i].use == true && _touches [i].id == Input.GetTouch (id).fingerId) {
						_touches [i].pos = Input.GetTouch (id).position;

						_inputs [i].state = TOUCH_HOLD;
						_inputs [i].pos = Input.GetTouch (id).position;
						return true;
					}
				}
			}
		} else {
			// mouse
			if (Input.GetMouseButton (0) == true) {
				_inputs [0].state = TOUCH_HOLD;
				_inputs [0].pos = Input.mousePosition;
				return true;
			}
		}

		return false;
	}

	bool touchEnd(int id) {
		if (Input.touchCount > id) {
			if (Input.GetTouch (id).phase == TouchPhase.Ended) {
				for (int i = 0; i < MAX_INPUT; i++) {
					if (_touches [i].use == true && _touches [i].id == Input.GetTouch (id).fingerId) {
						_touches [i].use = false;

						_inputs [i].state = TOUCH_UP;
						_inputs [i].pos = Input.GetTouch (id).position;
						return true;
					}
				}
			}
		} else {
			// mouse
			if (Input.GetMouseButtonUp (0) == true) {
				_inputs[0].state = TOUCH_UP;
				_inputs [0].pos = Input.mousePosition;
				return true;
			}
		}
		return false;
	}



}
