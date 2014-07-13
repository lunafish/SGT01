using UnityEngine;
using System.Collections;

public class avatar_cont : MonoBehaviour {
	public SkinnedMeshRenderer _arm_l_src;
	public SkinnedMeshRenderer _hand_l_src;
	public SkinnedMeshRenderer _arm_l_dst;
	public SkinnedMeshRenderer _hand_l_dst;


	// Use this for initialization
	void Start () {
		_arm_l_src.sharedMesh = _arm_l_dst.sharedMesh;
		_arm_l_src.material = _arm_l_dst.material;
		_hand_l_src.sharedMesh = _hand_l_dst.sharedMesh;
		_hand_l_src.material = _hand_l_dst.material;
		//_src.GetComponent<SkinnedMeshRenderer>().sharedMesh = _dst.GetComponent<SkinnedMeshRenderer>().sharedMesh;
		//_src.GetComponent<SkinnedMeshRenderer> ().material = _dst.GetComponent<SkinnedMeshRenderer> ().material;
	}
	
	// Update is called once per frame
	void Update () {

	}
}
