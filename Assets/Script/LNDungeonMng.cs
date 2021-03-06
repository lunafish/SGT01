﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LNDungeonMng : MonoBehaviour {
	public GameObject _player;
	public GameObject _camera;
	public LNHUDDungeon _hudDungeon;
	public int _max_corridor = 32;
	public int _max_x = 8;
	public int _max_y = 8;

	private float _tile_size = 24.0f; // tile size

//	private ArrayList _corridor = new ArrayList(); // dungeon corridor
	private Dictionary<int, GameObject> _corridor = new Dictionary<int, GameObject>();

	// gate test
	private bool _isGate = false;

	struct MASK {
		public bool[] _flag;

		public void set( bool up, bool down, bool left, bool right ) {
			_flag = new bool[4];
			_flag[0] = up;
			_flag[1] = down;
			_flag[2] = left;
			_flag[3] = right;
		}
	};
	private MASK[] _mask = new MASK[16];

	// Use this for initialization
	void Start () {
		// mask data
		// 4way
		_mask [0].set (true, true, true, true);

		// 3way
		_mask [1].set (false, true, true, true);
		_mask [2].set (true, false, true, true);
		_mask [3].set (true, true, false, true);
		_mask [4].set (true, true, true, false);

		// 2way
		_mask [5].set (false, false, true, true);
		_mask [6].set (true, false, false, true);
		_mask [7].set (true, true, false, false);
		_mask [8].set (false, true, false, false);
		_mask [9].set (true, false, true, false);
		_mask [10].set (false, true, true, false);
		_mask [11].set (true, false, false, true);

		// 1way
		_mask [12].set (true, false, false, false);
		_mask [13].set (false, true, false, false);
		_mask [14].set (false, false, true, false);
		_mask [15].set (false, false, false, true);
		//

		make_dungeon ( );

		// make hud dungeon map
		_hudDungeon.Make ();
	}
	
	// Update is called once per frame
	void Update () {

	}

	// get dungeon object in _corridor
	public GameObject Get_dungeon_object( int x, int y ) {
		int idx = x + (y * _max_x);
		if(_corridor.ContainsKey(idx) == true) {
			GameObject obj = _corridor[idx];
			return obj;
		}
		return null;
	}

	// get dungeon ctrl in _corridor
	LNDungeonCtrl get_dungen_ctrl( int x, int y ) {
		GameObject obj = Get_dungeon_object (x, y);
		if(obj) {
			return obj.GetComponent<LNDungeonCtrl>();
		}
		return null;
	}

	GameObject is_empty_room( int x, int y ) {
		int idx = x + (y * _max_x);
		if(_corridor.ContainsKey(idx) == true) {
			GameObject obj = _corridor[idx];
			return obj;
		}
		return null;
	}

	GameObject make_room( GameObject parent, int x, int y, int use ) {
		GameObject child = is_empty_room( x, y);
		if(child) {
			return null;
		}

		// make object
		GameObject obj = Instantiate(Resources.Load("prefabs/dungeon_map")) as GameObject;	
		_corridor.Add (x + (y * _max_x), obj);

		// get ctrl
		LNDungeonCtrl ctrl = obj.GetComponent<LNDungeonCtrl>();

		// init ctrl
		ctrl.Init ();

		ctrl._x = x;
		ctrl._y = y;

		if(use == 0) { ctrl._node[1] = parent;}
		else if(use == 1) { ctrl._node[0] = parent;}
		else if(use == 2) { ctrl._node[3] = parent;}
		else if(use == 3) { ctrl._node[2] = parent;}

		// set transform
		Vector3 pos = new Vector3 ((x - (_max_x/2)) * _tile_size, 0.0f, (y - (_max_y/2)) * _tile_size);
		obj.transform.position = pos;

		// test code
		/*
		if(parent != null) {
			GameObject enemy = Instantiate(Resources.Load("prefabs/pawn_00")) as GameObject;	
			enemy.transform.position = pos;
		}
		*/
		//

		return obj;
	}

	GameObject rec_make_dungeon( GameObject parent, int x, int y, int use ) {
		// end condition
		if (_corridor.Count > _max_corridor) {
			return null;
		}

		if(x < 0 || x >= _max_x) {
			return null;
		}

		if(y < 0 || y >= _max_y) {
			return null;
		}

		// root node
		if (parent == null) {
			x = Random.Range(_max_x / 4, _max_x / 2);
			y = Random.Range(_max_y / 4, _max_y / 2);
		}

		GameObject obj = make_room( parent, x, y, use );
		if(obj == null) {
			return null;
		}

		// player init postion
		if (parent == null) {
			_player.GetComponent<LNPlayerCtrl>().Move(obj.transform.position);
			_camera.transform.position = obj.transform.position;
		}


		LNDungeonCtrl ctrl = obj.GetComponent<LNDungeonCtrl>();
		GameObject child = null;
		bool noWay = true;
		for(int i = 0; i < 16; i++) {
			int n = Random.Range (0, 15);
			int count = 0;
			if (ctrl._node [0] == null && _mask[n]._flag[0]) {
				if( (y-1) >= 0 ) {
					if(child = rec_make_dungeon (obj, x, y-1, 0)) {
						ctrl._node[0] = child;
						count++;
					}
				}
			}
			if (ctrl._node [1] == null && _mask[n]._flag[1]) {
				if( (y+1) < _max_y ) {
					if(child = rec_make_dungeon (obj, x, y+1, 1)) {
						ctrl._node[1] = child;
						count++;
					}
				}
			}
			if (ctrl._node [2] == null && _mask[n]._flag[2]) {
				if( (x-1) >= 0 ) {
					if(child = rec_make_dungeon (obj, x+1, y, 2)) {
						ctrl._node[2] = child;
						count++;
					}
				}
			}
			if (ctrl._node [3] == null && _mask
			    [n]._flag[3]) {
				if( (x+1) < _max_x ) {
					if(child = rec_make_dungeon (obj, x-1, y, 3)) {
						ctrl._node[3] = child;
						count++;
					}
				}
			}

			if(count > 0)
			{
				noWay = false;
				break;
			}
		}

		ctrl.Road();
		if (noWay) {
			Debug.Log("Last node");
			// check near room
			bool chk = false;
			GameObject room;

			room = is_empty_room(ctrl._x, ctrl._y-1); //up
			if(room) {
				if(room.GetComponent<LNDungeonCtrl>()._isRoom == true)
					chk = true;
			}
			room = is_empty_room(ctrl._x, ctrl._y+1); //down
			if(room) {
				if(room.GetComponent<LNDungeonCtrl>()._isRoom == true)
					chk = true;
			}
			room = is_empty_room(ctrl._x-1, ctrl._y); //left
			if(room) {
				if(room.GetComponent<LNDungeonCtrl>()._isRoom == true)
					chk = true;
			}
			room = is_empty_room(ctrl._x+1, ctrl._y); //right
			if(room) {
				if(room.GetComponent<LNDungeonCtrl>()._isRoom == true)
					chk = true;
			}

			if(chk) {
				Debug.Log("Don't make room!");
			} else {
				ctrl.Room ( true );

				// test code
				if(_isGate == false ) {
					ctrl.Regen( LNDungeonCtrl.REGEN.Gate );
					_isGate = true;
				} else {
					ctrl.Regen( LNDungeonCtrl.REGEN.Item );
				}
			}
		} else {
			// test code
			if (parent != null) {
				ctrl.Regen( LNDungeonCtrl.REGEN.OneByOne );
			}
		}

		return obj;
	}

	void make_dungeon(  ) {
		rec_make_dungeon (null, 0, 0, 0);
	}
}
