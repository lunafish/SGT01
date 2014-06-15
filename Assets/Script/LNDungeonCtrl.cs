using UnityEngine;
using System.Collections;

public class LNDungeonCtrl : MonoBehaviour {
	public int _x, _y;
	public bool _isRoom = false;
	public GameObject[] _node = new GameObject[4];
	public GameObject[] _way = new GameObject[4];
	public GameObject[] _object_in = new GameObject[4]; // wall inside
	public GameObject[] _object_out = new GameObject[4]; // wall outside
	public GameObject[] _object_side = new GameObject[4]; // wall outside door
	public GameObject _object_wall; // wall
	public GameObject _room;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {

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

	// set room
	public void Room( bool isRoom ) {
		_isRoom = true;
		_room.SetActive (true);
		_object_wall.SetActive (true);
		int doorDir = 0;
		for(int i = 0; i < 4; i++) {
			_way[i].SetActive(true);
			_object_in[i].SetActive(false);
			_object_out[i].SetActive(false);
			if(_node[i] == null) {
				_object_side[i].SetActive(true);
			} else {
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

		// make event
		// current only gate
		GameObject gate = Instantiate(Resources.Load("prefabs/npc_gate")) as GameObject;	
		gate.GetComponent<LNAIPawn> ()._npc_target = "testStage";
		gate.transform.position = transform.position;
		gate.transform.Rotate(0.0f, 90.0f * doorDir, 0.0f);
		//
	}
}
