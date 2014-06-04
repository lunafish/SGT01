//
// For dynamic object (player, npc, monster...)
//

using UnityEngine;
using System.Collections;

public class LNPawn : MonoBehaviour {
	public float _speed = 1.0f; // default player move speed
	public float _rotate_speed = 1.0f; // default camera rotate speed
	public GameObject _avatar; // avatar object
	public ePawn _type; // pawn type

	public enum ePawn {
		PLAYER = 0,
		NPC,
		ENEMY,
	};

	public enum eAction {
		NONE = 0,
		NPC,
		ENEMY,
	};

	public float Range(GameObject target) {
		Vector3 v = transform.position - target.transform.position;
		return v.magnitude;
	}

	public virtual eAction Action(GameObject target) {
		return eAction.NONE;
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
}
