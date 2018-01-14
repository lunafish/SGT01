/** static utility lunafish 2014-07-22 **/
using UnityEngine;
using System.Collections;

public class LNUtil {
	private static LNUtil _instance = null; // singleton instance

	// return point
	private bool _isReturn = false; // set return point
	private int _returnLevel = -1;
	private Vector3 _returnPos = new Vector3();
	private Quaternion _returnRot = new Quaternion();

	// for current parts
	public string _armParts = ""; // arm parts
	public string _wpParts = ""; // weapon parts

	// for tutorial
	public bool _isTutorial = true;

	// text read from resource
	static public bool ReadText( string path, out string txt ) {
		TextAsset ta = (TextAsset)Resources.Load (path) as TextAsset;
		if (ta == null) {
			txt = "";
			return false;
		}
		txt = ta.text;
		return true;
	}

	// get singleton instance
	static public LNUtil Instance( ) { 
		if(_instance == null ) {
			Debug.Log("Make Singleton");
			_instance = new LNUtil(); // make instance
		}

		return _instance;
	}

	// set return point
	public void SetReturnPoint( Vector3 pos, Quaternion rot ) {
		_isReturn = true;
		_returnPos = pos;
		_returnRot = rot;
		_returnLevel = Application.loadedLevel;

		Debug.Log(Application.loadedLevelName);
		Debug.Log("SAVE POINT( " + _returnLevel + " ) : " + _returnPos);
	}

	// get return point (once)
	public bool GetReturnPoint( out Vector3 pos, out Quaternion rot ) {
		pos = _returnPos;
		rot = _returnRot;
		if(!_isReturn) {
			return false;
		}
		if(_returnLevel != Application.loadedLevel) {
			return false;
		}
		Debug.Log("GET POINT( " + _returnLevel + " ) : " + _returnPos);

		_isReturn = false;
		return true;
	}
}
