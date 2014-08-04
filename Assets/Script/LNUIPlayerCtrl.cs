/** Player UI controller lunafish 2014-08-04 **/
using UnityEngine;
using System.Collections;

public class LNUIPlayerCtrl : MonoBehaviour {
	public GameObject _hpBar; // hp bar
	public GameObject _mpBar; // mp bar
	public GameObject _hpGage; // hp gage;
	public GameObject _mpGage; // mp gage

	private GameObject _player = null; // player

	private float _maxHP, _maxMP; // max HP & MP
	private float _curHP, _curMP; // current HP & MP

	// Use this for initialization
	void Start () {
		// init postion
		float ratio = (float)Screen.width / (float)Screen.height;
		Vector3 pos = _hpBar.transform.position;
		pos.x = -ratio;
		_hpBar.transform.position = pos;
		pos.x = -ratio + 0.98f;
		_mpBar.transform.position = pos;

		_player = GameObject.FindGameObjectWithTag("Player");
		_maxHP = _player.GetComponent<LNPawn>().GetMaxHP();
		_maxMP = _player.GetComponent<LNPawn>().GetMaxMP();
	}
	
	// Update is called once per frame
	void Update () {
		if(_player != null) {
			updateProperty( );
		}
	}

	// update hp & mp
	void updateProperty( ) {
		_curHP = _player.GetComponent<LNPawn>()._hp;
		_curMP = _player.GetComponent<LNPawn>()._mp;

		float hp = _curHP / _maxHP;
		float mp = _curMP / _maxMP;

		_hpGage.transform.localScale = new Vector3(hp, 1.0f, 1.0f);
		_mpGage.transform.localScale = new Vector3(mp, 1.0f, 1.0f);
	}
}
