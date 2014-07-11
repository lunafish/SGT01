using UnityEngine;
using System.Collections;

public class LNDungeonCtrl : MonoBehaviour {
	public int _x, _y;
	public bool _isRoom = false;
	public GameObject[] _node = new GameObject[4];

	// for enemy & item regen
	public int _regen_count = 0;
	public int _regen_count_max = 3;
	public enum REGEN {
		OneByOne = 0,
		ItemLast,
		Item,
		Boss,
		Gate,
		All,
	}
	public REGEN _regen = REGEN.OneByOne;
	public float _regen_delta;
	public enum TRIGER {
		FIND = 0,
		ATTACK,
		DAMAGE,
		DIE,
	};
	public bool _is_regen = false;
	//

	// dynamic object list
	private ArrayList _lstPawn = new ArrayList ();

	// mesh object
	private GameObject[] _way = new GameObject[4];
	private GameObject[] _object_in = new GameObject[4]; // wall inside
	private GameObject[] _object_out = new GameObject[4]; // wall outside
	private GameObject[] _object_side = new GameObject[4]; // wall outside door
	private GameObject _object_wall; // wall
	private GameObject _room;
	//
	

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		updateRegen ();
		updatePawn();
	}

	public void Init( ) {
		_way [0] = transform.FindChild ("road_up").gameObject;
		_way [1] = transform.FindChild ("road_down").gameObject;
		_way [2] = transform.FindChild ("road_left").gameObject;
		_way [3] = transform.FindChild ("road_right").gameObject;
		
		_object_in [0] = transform.FindChild ("obj_in_up").gameObject;
		_object_in [1] = transform.FindChild ("obj_in_down").gameObject;
		_object_in [2] = transform.FindChild ("obj_in_left").gameObject;
		_object_in [3] = transform.FindChild ("obj_in_right").gameObject;
		
		_object_out [0] = transform.FindChild ("obj_out_up").gameObject;
		_object_out [1] = transform.FindChild ("obj_out_down").gameObject;
		_object_out [2] = transform.FindChild ("obj_out_left").gameObject;
		_object_out [3] = transform.FindChild ("obj_out_right").gameObject;
		
		_object_side [0] = transform.FindChild ("obj_wall_up").gameObject;
		_object_side [1] = transform.FindChild ("obj_wall_down").gameObject;
		_object_side [2] = transform.FindChild ("obj_wall_left").gameObject;
		_object_side [3] = transform.FindChild ("obj_wall_right").gameObject;

		_object_wall = transform.FindChild ("obj_wall").gameObject;

		_room = transform.FindChild ("road_room").gameObject;
	}

	// set road
	public void Road( ) {
		_room.SetActive (false);
		_object_wall.SetActive (false);
		for(int i = 0; i < 4; i++) {
			if(_node[i] == null) {
				_way[i].SetActive(false);
				_object_in[i].SetActive(true);
				_object_out[i].SetActive(false);
				_object_side[i].SetActive(false);
			} else {
				_way[i].SetActive(true);
				_object_in[i].SetActive(false);
				_object_out[i].SetActive(true);
				_object_side[i].SetActive(false);
			}
		}
	}

	int get_door_dir( ) {
		int doorDir = 0;
		for(int i = 0; i < 4; i++) {
			if(_node[i] != null) {
				if(i == 0)
					doorDir = 2; // up
				else if(i == 1)
					doorDir = 0; // down
				else if(i == 1)
					doorDir = 1; // left
				else if(i == 3)
					doorDir = 2; // right
			}
		}

		return doorDir;
	}

	// set room
	public void Room( bool isRoom ) {
		_isRoom = true;
		_room.SetActive (true);
		_object_wall.SetActive (true);
		for(int i = 0; i < 4; i++) {
			_way[i].SetActive(true);
			_object_in[i].SetActive(false);
			_object_out[i].SetActive(false);
			if(_node[i] == null) {
				_object_side[i].SetActive(true);
			}
		}
	}

	public void Regen( REGEN type ) {
		_regen = type;
		_is_regen = true;
	}

	// update monster & item regen
	void updateRegen( ) {
		if(_is_regen == false) {
			return;
		}

		switch (_regen) {
		case REGEN.OneByOne : 
			// gen summoner
			GameObject summ = Instantiate(Resources.Load("prefabs/summoner")) as GameObject; // regen
			// set transform
			summ.transform.position = transform.position + new Vector3(0.0f, 2.5f, 0.0f);
			summ.transform.Rotate(0.0f, 90.0f * get_door_dir(), 0.0f);
			summ.GetComponent<LNSummoner>()._corridor = transform.gameObject;
			//
			//summ.GetComponent<LNSummoner>().Regen("pawn_00"); // regen

			_is_regen = false; // set regen flag
			/*
			GameObject mon = Instantiate(Resources.Load("prefabs/pawn_00")) as GameObject;	
			mon.transform.position = transform.position;
			mon.transform.Rotate(0.0f, 90.0f * get_door_dir(), 0.0f);
			mon.GetComponent<LNAIPawn>()._regen = transform.gameObject; // set regen point
			_is_regen = false;
			*/
			break;
		case REGEN.ItemLast : 
			break;
		case REGEN.Item : 
			break;
		case REGEN.Boss : 
			break;
		case REGEN.Gate : 
			GameObject gate = Instantiate(Resources.Load("prefabs/npc_gate")) as GameObject;	
			gate.GetComponent<LNAIPawn> ()._npc_target = "TownStage";
			gate.transform.position = transform.position;
			gate.transform.Rotate(0.0f, 90.0f * get_door_dir(), 0.0f);
			gate.GetComponent<LNAIPawn>()._corridor = transform.gameObject; // set regen point
			_is_regen = false;
			break;
		case REGEN.All : 
			break;
		default :
			break;
		};	
	}

	// triger call back
	public void Triger( GameObject obj, TRIGER triger ) {
		Debug.Log ("(" + _x + ", " + + _y +  ") : " + triger);

	}

	// update pawn list
	void updatePawn() {
		for(int i = 0; i < _lstPawn.Count; i++) {
			GameObject obj = (GameObject)_lstPawn[i];
			if(obj.GetComponent<LNPawn>()._x != _x || obj.GetComponent<LNPawn>()._y != _y) {
				_lstPawn.RemoveAt( i  ); // one by one
				Debug.Log("Remove Pawn : " + obj + " " + _x + " : " + _y );
				return;
			}
		}
	}

	// add new pawn on tile
	public void AddPawn( GameObject pawn ) {
		for(int i = 0; i < _lstPawn.Count; i++) {
			if( (GameObject)_lstPawn[i] == pawn ) {
				return; // same object exist
			}
		}
		pawn.GetComponent<LNPawn> ()._x = _x;
		pawn.GetComponent<LNPawn> ()._y = _y;

		Debug.Log("Add Pawn : " + pawn + " "+ _x + " : " + _y );

		_lstPawn.Add (pawn);
	}
}
