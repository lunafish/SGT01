using UnityEngine;
using System.Collections;

public class LNDungeonMng : MonoBehaviour {
	public GameObject _player;
	public GameObject _camera;
	public int _max_room = 20;
	public int _max_x = 16;
	public int _max_y = 16;

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

	bool is_empty_room( int x, int y ) {
		for(int i = 0; i < _rooms.Count; i++) {
			GameObject obj = (GameObject)_rooms[i];
			LNDungeonCtrl ctrl = obj.GetComponent<LNDungeonCtrl>();
			if(ctrl._x == x && ctrl._y == y) {
				return false;
			}
		}
		return true;
	}

	GameObject make_room( GameObject parent, int x, int y, int use ) {
		if(!is_empty_room( x, y )) {
			return null;
		}

		// make object
		GameObject obj = Instantiate(Resources.Load("prefabs/dungeon_map")) as GameObject;	
		_rooms.Add (obj);

		// get ctrl
		LNDungeonCtrl ctrl = obj.GetComponent<LNDungeonCtrl>();
		ctrl._x = x;
		ctrl._y = y;
		ctrl._node [use] = parent;

		// set transform
		Vector3 pos = new Vector3 (x * 8.0f, 0.0f, y * 8.0f);
		obj.transform.position = pos;

		return obj;
	}

	bool rec_make_dungeon( GameObject parent, int x, int y, int use ) {
		// end condition
		if (_rooms.Count > _max_room) {
			return false;
		}

		// root node
		if (parent == null) {
			x = Random.Range(_max_x / 4, _max_x / 2);
			y = Random.Range(_max_y / 4, _max_y / 2);
		}

		GameObject obj = make_room( parent, x, y, use );
		if(obj == null) {
			return false;
		}

		// player init postion
		if (parent == null) {
			_player.transform.position = obj.transform.position;
			_camera.transform.position = obj.transform.position;
		}


		LNDungeonCtrl ctrl = obj.GetComponent<LNDungeonCtrl>();

		for(int i = 0; i < 16; i++) {
			int n = Random.Range (0, 15);
			int count = 0;
			if (ctrl._node [0] == null && _mask[n]._flag[0]) {
				if( (y-1) >= 0 ) {
					if(rec_make_dungeon (obj, x, y-1, 0)) {
						count++;
					}
				}
			}
			if (ctrl._node [1] == null && _mask[n]._flag[1]) {
				if( (y+1) < _max_y ) {
					if(rec_make_dungeon (obj, x, y+1, 1)) {
						count++;
					}
				}
			}
			if (ctrl._node [2] == null && _mask[n]._flag[2]) {
				if( (x-1) >= 0 ) {
					if(rec_make_dungeon (obj, x-1, y, 2)) {
						count++;
					}
				}
			}
			if (ctrl._node [3] == null && _mask
			    [n]._flag[3]) {
				if( (x+1) < _max_x ) {
					if(rec_make_dungeon (obj, x+1, y, 3)) {
						count++;
					}
				}
			}

			if(count > 0)
				break;

			Debug.Log("No Way");
		}

		return true;
	}

	void make_dungeon(  ) {
		rec_make_dungeon (null, 0, 0, 0);
	}
}
