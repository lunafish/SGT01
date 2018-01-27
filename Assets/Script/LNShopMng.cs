/** SHOP Managemant lunafish 2014-07-22 **/
using UnityEngine;
using SimpleJSON;
using System.Collections;

public class LNShopMng : MonoBehaviour {
	// for tab
	private GameObject[] _tabs = null; // tab object
	private GameObject _list = null; // item list object
	private string[] _tab_name = {"tab_arm", "tab_wp"}; // tab object name
	private string _current_tab;

	// for avatar
	public GameObject _avatar = null;

	// for json
	private JSONNode _json = null;

	// for layout
	public GameObject _btnBack = null;
	public TextMesh _info = null; // info

	// for UI sound
	private AudioSource _audio;
	public AudioClip _tabsound;
	public AudioClip _itemsound;

	// for pad
	private LNPad _pad = null;

	// Use this for initialization
	void Start () {
		init ();
	}
	
	// Update is called once per frame
	void Update () {
		//updateInput (); // update input
		updateUI (); // process input
	}

	// init ui data
	void init() {
		_tabs = GameObject.FindGameObjectsWithTag ("UITab");
		_list = GameObject.FindGameObjectWithTag("UIList"); // find list object
		selectTab (_tab_name[0]);

		// read json data
		string txt;
		if( LNUtil.ReadText ("json/shopdata", out txt)) {
			_json = JSONNode.Parse( txt );

			loadList(_tab_name[0]);
		}

		// init layout
		float ratio = (float)Screen.width / (float)Screen.height;

		// back button
		Vector3 pos = _btnBack.transform.position;
		pos.x = ratio - 0.16f;
		_btnBack.transform.position = pos;

		// tab & list
		// tab
		foreach( GameObject o in _tabs ) {
			pos = o.transform.position;
			pos.x = -ratio + 0.16f;
			o.transform.position = pos;
		}
		// list
		pos = _list.transform.position;
		pos.x = -ratio + 0.634f;
		_list.transform.position = pos;
		//


		// for audio
		_audio = GetComponent<AudioSource>();

		// find pad
		_pad = GameObject.FindGameObjectWithTag("Input").GetComponent<LNPad>();

	}

	void clearListItem( ) {
		GameObject[] items = GameObject.FindGameObjectsWithTag("UIListItem"); // get list

		foreach( GameObject obj in items ) {
			Destroy( obj.GetComponent<LNPartsData>()._parts ); // destory parts
			Destroy (obj );
		}

		// clear info
		_info.text = "";

	}

	// load tab list
	void loadList( string tab ) {
		_current_tab = tab;


		clearListItem(); // clear item

//		Debug.Log(tab);

		for(int i = 0; i < _json[tab].Count; i++ ) {
			GameObject item = Instantiate(Resources.Load("prefabs/list_item")) as GameObject; // list item
			
			// set transform
			item.transform.position = _list.transform.position;
			item.transform.parent = _list.transform;
			item.transform.localPosition = new Vector3(0.0f, 0.8f - (i * 0.32f), 1.0f);
			//

			item.GetComponent<LNPartsData>().Load( _json[tab][i] );
		}
	}

	// process ui
	void updateUI( ) {
		for(int i = 0; i < LNPad.MAX_INPUT; i++) {
			if(_pad.getInput(i).state == LNPad.TOUCH_DOWN) {
				GameObject obj = getUI( i );
				if(obj != null) {
					// process tab
					if(obj.tag.Equals("UITab")) {
						// sound play
						if(_tabsound != null) {
							_audio.clip = _tabsound;
							_audio.Play();
						}

						selectTab(obj.name);
					} else if(obj.tag.Equals("UIListItem")) {
						Debug.Log(obj.tag);

						// sound play
						if(_itemsound != null) {
							_audio.clip = _itemsound;
							_audio.Play();
						}

						// set infomation
						_info.text = obj.GetComponent<LNPartsData>()._info;
						//

						if(_current_tab.Equals(_tab_name[0])) {
							// change arm
							LNUtil.Instance()._armParts = obj.GetComponent<LNPartsData>()._prefabs;
							_avatar.GetComponent<LNPartsCtrl>().ChangeArm( obj.GetComponent<LNPartsData>()._parts ); // change parts
						} else {
							// change katana
							LNUtil.Instance()._wpParts = obj.GetComponent<LNPartsData>()._prefabs;
							_avatar.GetComponent<LNPartsCtrl>().ChangeKatana( obj.GetComponent<LNPartsData>()._parts ); // change parts
						}

					} else {
						_pad.SetRotate (i, true); // rotate avatar
					}
				} else {
					_pad.SetRotate (i, true); // rotate avatar
				}
			} else if(_pad.getInput(i).state == LNPad.TOUCH_HOLD) {
				if(_pad.getInput(i).rotate) {
					Vector3 v = _pad.getInput(i).pos - _pad.getInput(i).org_pos;
					rotate(v);
				}
			}
		}
	}

	// rotate
	void rotate(Vector3 v) {
		// check rotate bound
		if( Mathf.Abs( v.x ) > 16.0f && Mathf.Abs (v.y) < 64.0f) {
			float f = -1.0f * (v.x * 0.05f);
			_avatar.transform.Rotate(0.0f, f, 0.0f);
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

		if(_json != null) {
			loadList(tab); // load list
		}
	}

	// UI Check
	GameObject getUI( int touchIndex ) {
		// check ui
		Ray ray = Camera.main.ScreenPointToRay(_pad.getInput(touchIndex).org_pos); // get mouse ray
		//Debug.Log (ray);
		RaycastHit hit; // hit object
		if(Physics.Raycast (ray, out hit, Mathf.Infinity)) {
			return hit.transform.gameObject;
		}
		//

		return null;
	}

	// back button event
	void OnBackButton( ) {
		Application.LoadLevel("TownStage");
	}
}
