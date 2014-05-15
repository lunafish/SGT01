using UnityEngine;
using System.Collections;

public class LNDungeonMng : MonoBehaviour {
	private ArrayList _rooms = new ArrayList(); // dungeon rooms
	private int _max_room = 20;
	private int _max_x = 8;
	private int _max_y = 8;

	struct mask {
		public bool[] _flag;

		public void set( bool u, bool d, bool l, bool r ) {
			_flag = new bool[4];
			_flag[0] = u;
			_flag[1] = d;
			_flag[2] = l;
			_flag[3] = r;
		}
	};

	// Use this for initialization
	void Start () {
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

	bool rec_make_dungeon( GameObject parent, int x, int y, int use ) {
		// end condition
		if(!is_empty_room( x, y )) {
			return false;
		}
		if (_rooms.Count > _max_room) {
			return false;
		}

		GameObject obj = Instantiate(Resources.Load("prefabs/dungeon_map")) as GameObject;	
		_rooms.Add (obj);

		LNDungeonCtrl ctrl = obj.GetComponent<LNDungeonCtrl>();
		if (parent == null) {
			// root node
			x = Random.Range(0, _max_x);
			y = Random.Range(0, _max_y);
			x = ctrl._x;
			y = ctrl._y;
		} else {
			ctrl._x = x;
			ctrl._y = y;
			ctrl._node [use] = parent;
		}

		Vector3 pos = new Vector3 (x * 8.0f, 0.0f, y * 8.0f);
		obj.transform.position = pos;

		mask[] maskes = new mask[16];
		maskes [0].set (true, true, true, true);

		maskes [1].set (false, true, true, true);
		maskes [2].set (true, false, true, true);
		maskes [3].set (true, true, false, true);
		maskes [4].set (true, true, true, false);

		maskes [5].set (false, false, true, true);
		maskes [6].set (true, false, false, true);
		maskes [7].set (true, true, false, false);
		maskes [8].set (false, true, false, false);
		maskes [9].set (true, false, true, false);
		maskes [10].set (false, true, true, false);
		maskes [11].set (true, false, false, true);

		maskes [12].set (true, false, false, false);
		maskes [13].set (false, true, false, false);
		maskes [14].set (false, false, true, false);
		maskes [15].set (false, false, false, true);

		for(int i = 0; i < 16; i++) {
			int n = Random.Range (0, 15);
			int count = 0;
			if (ctrl._node [0] == null && maskes[n]._flag[0]) {
				if( (y-1) >= 0 ) {
					if(rec_make_dungeon (obj, x, y-1, 0)) {
						count++;
					}
				}
			}
			if (ctrl._node [1] == null && maskes[n]._flag[1]) {
				if( (y+1) < _max_y ) {
					if(rec_make_dungeon (obj, x, y+1, 1)) {
						count++;
					}
				}
			}
			if (ctrl._node [2] == null && maskes[n]._flag[2]) {
				if( (x-1) >= 0 ) {
					if(rec_make_dungeon (obj, x-1, y, 2)) {
						count++;
					}
				}
			}
			if (ctrl._node [3] == null && maskes[n]._flag[3]) {
				if( (x+1) < _max_x ) {
					if(rec_make_dungeon (obj, x+1, y, 3)) {
						count++;
					}
				}
			}

			if(count > 0)
				break;
		}

		return true;
	}

	void make_dungeon(  ) {
		rec_make_dungeon (null, 0, 0, 0);
	}
}
