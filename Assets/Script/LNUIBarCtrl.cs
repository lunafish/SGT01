/** Bar UI Controller lunafish 2014-07-13 **/
using UnityEngine;
using System.Collections;

public class LNUIBarCtrl : MonoBehaviour {
	public GameObject _border;
	public GameObject _bar;
	public float _range; // input value range
	public float _margin = 0.12f; // bar move margin


	// Use this for initialization
	void Start () {
		_range = 100.0f;
	}
	
	// Update is called once per frame
	void Update () {

	
	}

	// set value
	public void SetValue( float value ) {
		// wrong value
		if(value > _range) {
			return; 
		}
		float v = value / _range; // normailze value

		_bar.transform.localScale = new Vector3 (v, 1.0f, 1.0f);
		_bar.transform.localPosition = new Vector3 ((v- 1.0f) * _margin, 0.0f, 0.0f);
	}
}
