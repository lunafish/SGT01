using UnityEngine;
using System.Collections;

public class LNAIPawn : LNPawn {
	enum eEMOTION {
		ALERT = 0,
		TARGET,
		TALK,
		NONE,
		MAX,
	};
	public GameObject[] _emotions = new GameObject[(int)eEMOTION.MAX]; // emotion sprite
	eEMOTION _current_emotion = eEMOTION.ALERT; // set not init value

	// Use this for initialization
	void Start () {
		Emotion (eEMOTION.NONE);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void Emotion( eEMOTION e ) {
		if(_current_emotion == e )
			return;

		for(int i = 0; i < (int)eEMOTION.MAX; i++) {
			// set emotion
			if(e == (eEMOTION)i)
				_current_emotion = e;

			if(_emotions != null) {
				if(e == (eEMOTION)i ) {
					// correct emotion!
					_emotions[i].SetActive(true);
				}
				else {
					_emotions[i].SetActive(false);
				}
			}
		}
	}

	// targeting action
	public override void Target(GameObject target) {
		base.Target (target); // call parents method
		if(target == null) {
			Emotion (eEMOTION.NONE);
		} else {
			Emotion (eEMOTION.TARGET);
		}
	}
}
