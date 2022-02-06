using UnityEngine;

public class Ragdoll: MonoBehaviour{
	public bool simulate = false;

	public void updateSimulationFlag(){
		var children = GetComponentsInChildren<RagdollPart>();
		foreach(var cur in children){
			cur.updateSimulationFlag();
		}
	}
}