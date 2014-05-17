using UnityEngine;
using System.Collections;

public class LNDungeonMng : MonoBehaviour {
	public GameObject _player;
	public GameObject _camera;
	public int _max_room = 32;
	public int _max_x = 8;
	public int _max_y = 8;

	private ArrayList _rooms = new ArrayList(); // dungeon rooms

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
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	GameObject is_empty_room( int x, int y ) {
		for(int i = 0; i < _rooms.Count; i++) {
			GameObject obj = (GameObject)_rooms[i];
			LNDungeonCtrl ctrl = obj.GetComponent<LNDungeonCtrl>();
			if(ctrl._x == x && ctrl._y == y) {
				return obj;
			}
		}
		return null;
	}

	GameObject make_room( GameObject parent, int x, int y, int use ) {
		GameObject child = is_empty_room( x, y );
		if(child) {
//			parent.GetComponent<LNDungeonCtrl>()._node[use] = child;
//			parent.GetComponent<LNDungeonCtrl>().update_room();
			return null;
		}

		// make object
		GameObject obj = Instantiate(Resources.Load("prefabs/dungeon_map")) as GameObject;	
		_rooms.Add (obj);

		// get ctrl
		LNDungeonCtrl ctrl = obj.GetComponent<LNDungeonCtrl>();
		ctrl._x = x;
		ctrl._y = y;

		if(use == 0) { ctrl._node[1] = parent;}
		else if(use == 1) { ctrl._node[0] = parent;}
		else if(use == 2) { ctrl._node[3] = parent;}
		else if(use == 3) { ctrl._node[2] = parent;}

		// set transform
		Vector3 pos = new Vector3 (x * 12.0f, 0.0f, y * 12.0f);
		obj.transform.position = pos;

		return obj;
	}

	GameObject rec_make_dungeon( GameObject parent, int x, int y, int use ) {
		// end condition
		if (_rooms.Count > _max_room) {
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
			_player.transform.position = obj.transform.position;
			_camera.transform.position = obj.transform.position;
		}


		LNDungeonCtrl ctrl = obj.GetComponent<LNDungeonCtrl>();
		GameObject child = null;
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
				break;
			}
		}

		ctrl.update_room();

		return obj;
	}

	void make_dungeon(  ) {
		rec_make_dungeon (null, 0, 0, 0);
	}
}
