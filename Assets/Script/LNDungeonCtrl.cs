using UnityEngine;
using System.Collections;

public class LNDungeonCtrl : MonoBehaviour {
	public int _x, _y;
	public GameObject[] _node = new GameObject[4];
	public GameObject[] _way = new GameObject[4];

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void update_room( ) {
		for(int i = 0; i < 4; i++) {
			if(_node[i] == null) {
				_way[i].SetActive(false);
			} else {
				_way[i].SetActive(true);
			}
		}
	}
}
