using UnityEngine;

public class WoundCharacter: MonoBehaviour{
	public void applyWound(GameObject obj, Vector3 pos, Vector3 normal, float radius, float depth){
		var woundChar = obj.GetComponentInParent<WoundCharacter>();
		if (woundChar != this){
			Debug.Log("Wrong character");
			return;
		}

		var clipSphere = GetComponentInChildren<ClipSphere>();
		if (!clipSphere){
			Debug.Log("Clip sphere not found");
			return;
		}

		clipSphere.transform.position = pos;
		clipSphere.transform.localScale = new Vector3(radius, radius, depth);
		clipSphere.transform.rotation = Quaternion.LookRotation(-normal, Vector3.up);
	}
}
