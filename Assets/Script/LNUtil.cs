/** static utility lunafish 2014-07-22 **/
using UnityEngine;
using System.Collections;

public class LNUtil {
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
}
