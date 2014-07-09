/** Effect controller lunafish 2014-07-09 **/
using UnityEngine;
using System.Collections;

public class LNEffectCtrl : MonoBehaviour {
	public GameObject[] _effects; // effect sprite array
	public float _delay = 0.1f; // effect play delay

	protected float _nextDelay = 0.0f;
	protected int _nextIndex = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		updateEffect();
	}

	// effect update
	void updateEffect( ) {
		if(_nextIndex >= _effects.Length)
			return;

		_nextDelay += Time.deltaTime;
		if(_nextDelay > _delay) {
			playEffect( _nextIndex );
			_nextIndex++;
		}
	}

	void playEffect( int index ) {
		GameObject obj = _effects [index];
		obj.SetActive(true);
		obj.GetComponent<tk2dSpriteAnimator> ().Play ();
		_nextDelay = 0.0f;
	}

	// play effect
	public void Play( ) {
		// hide all
		for(int i = 1;  i < _effects.Length; i++) {
			_effects[i].SetActive(false);
		}
		_nextIndex = 0;

		playEffect (0); // play first
		_nextIndex++; // increase index
	}
}
