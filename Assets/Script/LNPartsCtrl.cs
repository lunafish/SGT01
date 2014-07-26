/** Avatar Parts Controller lunafish 2014-07-21 **/
using UnityEngine;
using System.Collections;

public class LNPartsCtrl : MonoBehaviour {
	public GameObject _avatar = null; // avatar
	public GameObject _target = null; // target parts object (package)

	public enum ePART {
		LARM = 0,
		LHAND,
		RWEAPON,
	};

	// parts
	public GameObject _parts_arm;
	public GameObject _parts_hand;
	public GameObject _parts_wp;

	// Use this for initialization
	void Start () {
		init ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// init avatar parts info
	void init( ) {
		Transform[] ts = _avatar.GetComponentsInChildren<Transform> ();
		foreach( Transform t in ts ) {
			if(t.gameObject.tag.Equals("PartsLArm")) {
				_parts_arm = t.gameObject;
			} else if(t.gameObject.tag.Equals("PartsLHand")) {
				_parts_hand = t.gameObject;
			} else if(t.gameObject.tag.Equals("PartsRWP")) {
				_parts_wp = t.gameObject;
			}
		}

		// load default parts
		// load arm
		if(LNUtil.Instance()._armParts.Length > 0 ) {
			Debug.Log("Arm : " + LNUtil.Instance()._armParts);
			GameObject obj = Instantiate(Resources.Load("prefabs/"+LNUtil.Instance()._armParts)) as GameObject;
			if(obj) {
				ChangeArm( obj );
				Destroy( obj );
			}
		}
		// load weapon
		if(LNUtil.Instance()._wpParts.Length > 0 ) {
			Debug.Log("WP : " + LNUtil.Instance()._wpParts);
			GameObject obj = Instantiate(Resources.Load("prefabs/"+LNUtil.Instance()._wpParts)) as GameObject;
			if(obj) {
				ChangeKatana( obj );
				Destroy( obj );
			}
		}
		//
	}

	// change skinned parts
	void changePart( GameObject src, GameObject tar ) {
		src.GetComponent<SkinnedMeshRenderer> ().sharedMesh = tar.GetComponent<SkinnedMeshRenderer> ().sharedMesh; // change mesh
		src.GetComponent<SkinnedMeshRenderer> ().material = tar.GetComponent<SkinnedMeshRenderer> ().material; // change material
	}

	// change Arm
	public void ChangeArm( GameObject parts ) {
		_target = parts;
		Change (ePART.LARM);
		Change (ePART.LHAND);
	}

	// change katana
	public void ChangeKatana( GameObject parts ) {
		_target = parts;
		Change (ePART.RWEAPON);
	}

	// Change Parts
	void Change( ePART tag ) {
		GameObject part = null;

		// make tag string
		string parttag = "PartsLArm";
		if( tag == ePART.LARM ) {
			parttag = "PartsLArm";
		} else if( tag == ePART.LHAND ) {
			parttag = "PartsLHand";
		} else if( tag == ePART.RWEAPON ) {
			parttag = "PartsRWP";
		}

		Debug.Log (parttag);

		// find target part
		Transform[] ts = _target.GetComponentsInChildren<Transform> ();
		foreach( Transform t in ts ) {
			if(t.gameObject.tag.Equals(parttag)) {
				part = t.gameObject;
				Debug.Log("FIND" + part);
				break;
			} 
		}

		// change part
		if (part != null) {
			Debug.Log("CHANGE" + part);
			if(tag == ePART.LARM) {
				changePart(_parts_arm, part);
			} else if(tag == ePART.LHAND) {
				changePart(_parts_hand, part);
			} else if(tag == ePART.RWEAPON) {
				changePart(_parts_wp, part);
			}
		}
	}
}
