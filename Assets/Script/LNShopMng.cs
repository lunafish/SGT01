/** SHOP Managemant lunafish 2014-07-22 **/
using UnityEngine;
using System.Collections;

public class LNShopMng : MonoBehaviour {
	// for INPUT
	// for mouse
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
#if UNITY_IPHONE
	private LNTouch[] _touches = new LNTouch[MAX_INPUT];
#endif

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
	//

	// for tab
	private GameObject[] _tabs = null;

	// for avatar
	public GameObject _avatar = null;

	// Use this for initialization
	void Start () {
		init ();
	}
	
	// Update is called once per frame
	void Update () {
		updateInput (); // update input
		updateUI (); // process input
	}

	// init ui data
	void init() {
		_tabs = GameObject.FindGameObjectsWithTag ("UITab");
		selectTab ("tab_arm");
	}

	// process ui
	void updateUI( ) {
		for(int i = 0; i < MAX_INPUT; i++) {
			if(_inputs[i].use == true) {
				if(_inputs[i].state == TOUCH_DOWN) {
					GameObject obj = getUI( i );
					if(obj != null) {
						// process tab
						if(obj.tag.Equals("UITab")) {
							selectTab(obj.name);
						} else if(obj.tag.Equals("UIListItem")) {
							Debug.Log(obj.tag);
						} 
					}
				}
			}
		}
	}

	// for tab
	void selectTab( string tab ) {
		foreach( GameObject o in _tabs ) {
			if(o.name.Equals(tab)) {
				o.GetComponent<tk2dSprite>().SetSprite("tab_select");
			} else {
				o.GetComponent<tk2dSprite>().SetSprite("tab_deselect");
			}
		}
	}

	// Player translate
	void updateInput( ) {
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

	// UI Check
	GameObject getUI( int touchIndex ) {
		// check ui
		Ray ray = Camera.main.ScreenPointToRay(_inputs[touchIndex].org_pos); // get mouse ray
		//Debug.Log (ray);
		RaycastHit hit; // hit object
		if(Physics.Raycast (ray, out hit, Mathf.Infinity)) {
			return hit.transform.gameObject;
		}
		//

		return null;
	}
}
