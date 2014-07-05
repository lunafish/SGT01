/** Dungeon Map Draw HUD lunafish 2014-06-27 **/
using UnityEngine;
using System.Collections;
using System.Collections.Generic; // for Dictionary

public class LNHUDDungeon : MonoBehaviour {
	public GameObject _player; // player
	public LNDungeonMng _dungeonmng; // dungeon manager
	public GameObject _map;
	public GameObject _mapButton;
	public GameObject _icon;

	private Dictionary<int, ICON> _icons = new Dictionary<int, ICON>();
	private int _player_pos = -1; // player index
	private int _player_index = 0; // player postion tile index
	private GameObject[] _pawns = null; // pawn list
	private float _update_delta = 0.0f; // update time

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
		UpdateNPC ();

		// test
		float ratio = (float)Screen.width / (float)Screen.height;
		Vector3 pos = _mapButton.transform.position;
		pos.x = ratio - 0.16f;
		_mapButton.transform.position = pos;

	}

	// make dungeon map
	public void Make( ) {
		_icon.SetActive (true);
		draw_dungeon_map ();
		_icon.SetActive (false);
	}

	// Update Player
	void UpdatePlayer( ) {
		int key = _player.GetComponent<LNPlayerCtrl>()._x + (_player.GetComponent<LNPlayerCtrl>()._y * _dungeonmng._max_x);
		int index = _player_index;
		if(_player.GetComponent<LNPlayerCtrl> ()._corridor != null) { 
			Vector3 v = _player.transform.position - _player.GetComponent<LNPlayerCtrl> ()._corridor.transform.position;
			int x = (int)(v.x / 4.0f);
			int y =  (int)(v.z / 4.0f);
			//Debug.Log(x + " : " + y);

			if(x == 0 && y == 0)
				index = 0; // center;
			else if(x == 0 && y == -1)
				index = 1; // up;
			else if(x == 0 && y == 1)
				index = 2; // down;
			else if(x == 1 && y == 0)
				index = 3; // left;
			else if(x == -1 && y == 0)
				index = 4; // right;
			else if(x == 1 && y == -1)
				index = 5; // upleft;
			else if(x == -1 && y == -1)
				index = 6; // upright;
			else if(x == 1 && y == 1)
				index = 7; // downleft;
			else if(x == -1 && y == 1)
				index = 8; // downright;
		}

		if(key != _player_pos || index != _player_index) {
			Debug.Log("Update Player : " + key );
			UpdateIcon(_player_pos, _player_index, "icon_tile");
			UpdateIcon(key, index, "icon_me");
			_player_pos = key;
			_player_index = index;
		}
	}

	void UpdateNPC( ) {
		_update_delta -= Time.deltaTime;
		if (_update_delta > 0.0f)
			return;
		_update_delta = 1.0f; // update time 1sec

		_pawns = GameObject.FindGameObjectsWithTag ("Pawn");

		// test code check gate
		for(int i = 0; i < _pawns.Length; i++) {
			LNAIPawn pawn = _pawns[i].GetComponent<LNAIPawn>();
			if(pawn) {
				int pos = pawn._x + (pawn._y * _dungeonmng._max_x);
				if(pawn._npc == LNAIPawn.eNPC.GATE) {
					UpdateIcon(pos, 0, "icon_terminal");
				}
			}
		}
		//

	}


	// update map icon
	public void UpdateIcon(int key, int index, string name ) {
		if(_icons.ContainsKey(key) == true) {
			ICON icon = _icons[key];
			GameObject tile = (GameObject)icon._icons[index];
			tile.GetComponent<tk2dSprite>().SetSprite(name);
		}	
	}

	GameObject make_sprite( float x, float y, ICON icon, LNDungeonCtrl ctrl, int index ) {
		GameObject obj = (GameObject)Instantiate (_icon);
		obj.transform.position = new Vector3 (x, y, 0) + _map.transform.position;
		obj.transform.parent = _map.transform;
		if (index == 0) {
			obj.SetActive(true);
		} else if (index < 5) {
			if(ctrl._node[index-1] == null && ctrl._isRoom == false) {
				obj.SetActive(false);
			}
		} else {
			if(ctrl._isRoom == false)
				obj.SetActive(false);
		}
		icon._icons.Add (obj);

		return obj;
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

		make_sprite (off_x, off_y, icon, ctrl, 0); // center
		make_sprite (off_x, -offset + off_y, icon, ctrl, 1); // up
		make_sprite (off_x, offset + off_y, icon, ctrl, 2); // down
		make_sprite (offset + off_x, off_y, icon, ctrl, 3); // left
		make_sprite (-offset + off_x, off_y, icon, ctrl, 4); // right
		make_sprite (offset + off_x, -offset + off_y, icon, ctrl, 5); // upleft
		make_sprite (-offset + off_x, -offset + off_y, icon, ctrl, 6); // upright
		make_sprite (offset + off_x, offset + off_y, icon, ctrl, 7); // downleft
		make_sprite (-offset + off_x, offset + off_y, icon, ctrl, 8); // downright

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

	// button event
	void OnMapButton( ) {
		_map.SetActive (!_map.activeSelf);
	}
}
