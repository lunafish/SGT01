/** Dungeon Map Draw HUD lunafish 2014-06-27 **/
using UnityEngine;
using System.Collections;
using System.Collections.Generic; // for Dictionary

public class LNHUDDungeon : MonoBehaviour {
	public GameObject _player; // player
	public LNDungeonMng _dungeonmng; // dungeon manager
	public GameObject _map;
	public GameObject _icon;

	private Dictionary<int, ICON> _icons = new Dictionary<int, ICON>();
	private int _player_pos = -1; // player index

	struct ICON {
		public int _x, _y;
		public ArrayList _icons;
	};


	// Use this for initialization
	void Start () {
		// map draw center
		_map.transform.position -= new Vector3( (float)(_dungeonmng._max_x / 2) * 0.24f, (float)(_dungeonmng._max_y / 2) * 0.24f, 0.0f);
	}
	
	// Update is called once per frame
	void Update () {
		UpdatePlayer ();
	}

	// Update Player
	void UpdatePlayer( ) {
		int key = _player.GetComponent<LNPlayerCtrl>()._x + (_player.GetComponent<LNPlayerCtrl>()._y * _dungeonmng._max_x);
		if(key != _player_pos) {
			Debug.Log("Update Player : " + key );
			UpdateIcon(_player_pos, "icon_tile");
			UpdateIcon(key, "icon_me");
			_player_pos = key;
		}
	}

	// make dungeon map
	public void Make( ) {
		_icon.SetActive (true);
		draw_dungeon_map ();
		_icon.SetActive (false);
	}

	// update map icon
	public void UpdateIcon(int key, string name ) {
		if(_icons.ContainsKey(key) == true) {
			ICON icon = _icons[key];
			GameObject tile = (GameObject)icon._icons[0];
			tile.GetComponent<tk2dSprite>().SetSprite(name);
		}	
	}

	// make dungeon icon
	ICON make_icon( int x, int y, LNDungeonCtrl ctrl ) {
		ICON icon = new ICON();
		icon._icons = new ArrayList ();
		icon._x = x;
		icon._y = y;
		float offset = 0.08f;
		float off_x = (float)x * 0.24f;
		float off_y = (float)y * 0.24f;

		GameObject obj = null;
		// center
		obj = (GameObject)Instantiate (_icon);
		obj.transform.position = new Vector3(off_x, off_y, 0) + _map.transform.position;
		obj.transform.parent = _map.transform;
		icon._icons.Add (obj);
		// up
		obj = (GameObject)Instantiate (_icon);
		obj.transform.position = new Vector3 (off_x, -offset + off_y, 0) + _map.transform.position;
		obj.transform.parent = _map.transform;
		icon._icons.Add (obj);
		if(ctrl._node[0] == null && ctrl._isRoom == false) {
			obj.SetActive(false);
		}
		// down
		obj = (GameObject)Instantiate (_icon);
		obj.transform.position = new Vector3 (off_x, offset + off_y, 0) + _map.transform.position;
		obj.transform.parent = _map.transform;
		icon._icons.Add (obj);
		if(ctrl._node[1] == null && ctrl._isRoom == false) {
			obj.SetActive(false);
		}
		// left
		obj = (GameObject)Instantiate (_icon);
		obj.transform.position = new Vector3 (offset + off_x, off_y, 0) + _map.transform.position;
		obj.transform.parent = _map.transform;
		icon._icons.Add (obj);
		if(ctrl._node[2] == null && ctrl._isRoom == false) {
			obj.SetActive(false);
		}
		// right
		obj = (GameObject)Instantiate (_icon);
		obj.transform.position = new Vector3 (-offset + off_x, off_y, 0) + _map.transform.position;
		obj.transform.parent = _map.transform;
		icon._icons.Add (obj);
		if(ctrl._node[3] == null && ctrl._isRoom == false) {
			obj.SetActive(false);
		}
		if(ctrl._isRoom == true) {
			// upleft
			obj = (GameObject)Instantiate (_icon);
			obj.transform.position = new Vector3 (offset + off_x, -offset + off_y, 0) + _map.transform.position;
			obj.transform.parent = _map.transform;
			icon._icons.Add (obj);
			// upright
			obj = (GameObject)Instantiate (_icon);
			obj.transform.position = new Vector3 (-offset + off_x, -offset + off_y, 0) + _map.transform.position;
			obj.transform.parent = _map.transform;
			icon._icons.Add (obj);
			// downleft
			obj = (GameObject)Instantiate (_icon);
			obj.transform.position = new Vector3 (offset + off_x, offset + off_y, 0) + _map.transform.position;
			obj.transform.parent = _map.transform;
			icon._icons.Add (obj);		
			// downright
			obj = (GameObject)Instantiate (_icon);
			obj.transform.position = new Vector3 (-offset + off_x, offset + off_y, 0) + _map.transform.position;
			obj.transform.parent = _map.transform;
			icon._icons.Add (obj);
		}

		return icon;
	}

	// draw all Dungeon map
	void draw_dungeon_map( ) {
		for(int i = 0; i < _dungeonmng._max_y; i++) {
			for(int j = 0; j < _dungeonmng._max_x; j++) {
				GameObject obj = _dungeonmng.Get_dungeon_object(j, i);
				if(obj != null) {
					ICON icon = make_icon(j, i, obj.GetComponent<LNDungeonCtrl>());

					_icons.Add(j + (i * _dungeonmng._max_x), icon);
				}
			}
		}
	}
}
