using UnityEngine;
using System.Collections;

public class LNRule : MonoBehaviour {
	private GameObject[] _pawns;

	// Use this for initialization
	void Start () {
		InitPawn ();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// Init Pawn
	void InitPawn( ) {
		// make dynamic pawns

		// make pawn list
		_pawns = GameObject.FindGameObjectsWithTag ("Pawn");
	}
}
