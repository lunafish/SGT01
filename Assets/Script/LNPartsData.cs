/** Parts Data lunafish 2014-07-22 **/
using UnityEngine;
using SimpleJSON;
using System.Collections;

public class LNPartsData : MonoBehaviour {
	public GameObject _parts = null; // parts data object
	public tk2dSprite _icon = null;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// load data from json node
	public void Load( JSONNode json ) { 
		string sPrefabs = json["prefabs"];
		string sIcon = json["icon"];
		Debug.Log(sPrefabs);
		Debug.Log(sIcon);
		
		_icon.SetSprite(sIcon); // set icon
		_parts = Instantiate(Resources.Load("prefabs/"+sPrefabs)) as GameObject; // load parts
	}
}
