using UnityEngine;

using static GizmoTools;

[ExecuteInEditMode]
public class ColliderVisualizer: MonoBehaviour{
	BoxCollider[] boxes = new BoxCollider[0];
	void drawGizmo(Color c){
		var oldColor = Gizmos.color;
		Gizmos.color = c;

		foreach(var box in boxes){
			var center = box.center;
			var size = box.size;
			var pos = transform.TransformPoint(center);
			var dx = transform.TransformVector(new Vector3(size.x*0.5f, 0.0f, 0.0f));
			var dy = transform.TransformVector(new Vector3(0.0f, size.y*0.5f, 0.0f));
			var dz = transform.TransformVector(new Vector3(0.0f, 0.0f, size.z*0.5f));
			drawBox(pos, dx, dy, dz);
		}

		Gizmos.color = oldColor;
	}

	void OnDrawGizmos(){
		drawGizmo(Color.yellow);
	}

	void OnDrawGizmosSelected(){
		drawGizmo(Color.white);
	}

	void OnEnable(){
		boxes = GetComponents<BoxCollider>();
	}	
}