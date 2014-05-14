using UnityEngine;
using System.Collections;

public class avatar_cont : MonoBehaviour {
	private Vector3 _pos_start;
	private Vector3 _pos;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
#if UNITY_IPHONE
		if (Input.touchCount > 0 ) {
			if(Input.touches[0].phase == TouchPhase.Began) {
				_pos_start = Input.touches[0].position;
				_pos = _pos_start;
			}
			else if(Input.touches[0].phase == TouchPhase.Moved) {
				_pos = Input.touches[0].position;
				Vector3 v = _pos - _pos_start;
				
				if( Mathf.Abs( v.x ) > 10.0f )
				{
					transform.Rotate(0.0f, v.x * -0.1f, 0.0f);
				}
				
				if( Mathf.Abs( v.y ) > 10.0f )
				{
					Vector3 vec = new Vector3(0.0f, 0.0f, 1.0f);
					// don't change direction
					vec = Quaternion.Euler(0, -transform.rotation.eulerAngles.y, 0) * vec;
					vec.Normalize();
					vec = vec * (v.y * 0.001f);
					//
					transform.Translate(vec);
				}
			}
			else if(Input.touches[0].phase == TouchPhase.Ended) {
			}
		}
#else
		if(Input.GetMouseButtonDown(0) == true) {
			_pos_start = Input.mousePosition;
			_pos = _pos_start;
		}
		else if(Input.GetMouseButton(0) == true) {
			_pos = Input.mousePosition;
			Vector3 v = _pos - _pos_start;

			if( Mathf.Abs( v.x ) > 10.0f )
			{
				transform.Rotate(0.0f, v.x * -0.1f, 0.0f);
			}

			if( Mathf.Abs( v.y ) > 10.0f )
			{
				Vector3 vec = new Vector3(0.0f, 0.0f, 1.0f);
				// don't change direction
				vec = Quaternion.Euler(0, -transform.rotation.eulerAngles.y, 0) * vec;
				vec.Normalize();
				vec = vec * (v.y * 0.001f);
				//
				transform.Translate(vec);
			}
		}
		else if (Input.GetMouseButtonUp(0) == true) {
		}
#endif
	}
}
