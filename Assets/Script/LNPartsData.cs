/** Parts Data lunafish 2014-07-22 **/
using UnityEngine;
using SimpleJSON;
using System.Collections;

public class LNPartsData : MonoBehaviour {
	public GameObject _parts = null; // parts data object
	public TextMesh _label = null;
	public string _info = "";

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// load data from json node
	public void Load( JSONNode json ) { 
		string sPrefabs = json["prefabs"];
		string sName = json["name"];
		_info = json["info"];

		_label.text = sName;
		_parts = Instantiate(Resources.Load("prefabs/"+sPrefabs)) as GameObject; // load parts
	}
}
