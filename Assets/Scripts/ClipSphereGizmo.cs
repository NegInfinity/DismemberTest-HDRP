using UnityEngine;

using static GizmoTools;

public class ClipSphereGizmo: MonoBehaviour{
	[SerializeField] int numSegments = 16;
	
	void drawGizmos(){
		var pos = Vector3.zero;
		var x = Vector3.right;
		var y = Vector3.up;
		var z = Vector3.forward;
		drawCircle(transform, pos, x, y, 16);
		drawCircle(transform, pos, x, z, 16);
		drawCircle(transform, pos, y, z, 16);
	}

	void drawGizmos(Color c){
		var oldC = Gizmos.color;
		Gizmos.color = c;
		drawGizmos();
		Gizmos.color = oldC;
	}

	void OnDrawGizmos(){
		drawGizmos(Color.yellow);
	}

	void OnDrawGizmosSelected(){
		drawGizmos(Color.white);
	}
}