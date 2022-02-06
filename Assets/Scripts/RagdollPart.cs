using UnityEngine;

using static GizmoTools;

public class RagdollPart: MonoBehaviour{
	public Transform targetBone;
	public Pose originalPose;
	public Pose colliderPose;
	public Vector3 colliderBoxSize;
	Ragdoll ragdoll;

	void OnEnable(){
		ragdoll = GetComponentInParent<Ragdoll>();
	}

	void drawGizmosBody(){
		var pos = originalPose.position;
		var rot = originalPose.rotation;

		if (transform.parent){
			pos = transform.parent.TransformPoint(pos);
			rot = transform.parent.rotation * rot;
		}

		drawBox(pos, rot, colliderBoxSize);
	}

	void drawGizmos(Color c){
		var oldC  = Gizmos.color;
		Gizmos.color = c;
		drawGizmosBody();
		Gizmos.color = oldC;
	}

	void OnDrawGizmos(){
		drawGizmos(Color.yellow);
	}

	void OnDrawGizmosSelected(){
		drawGizmos(Color.white);
	}
}
