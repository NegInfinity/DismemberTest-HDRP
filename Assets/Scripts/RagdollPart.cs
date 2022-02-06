using UnityEngine;

using static GizmoTools;

public class RagdollPart: MonoBehaviour{
	public Transform targetBone;
	public Pose originalPose;
	public Vector3 colliderBoxSize;
	public Vector3 colliderBoxCenter;
	Ragdoll ragdoll;
	Rigidbody rigBody;

	void OnEnable(){
		ragdoll = GetComponentInParent<Ragdoll>();
		TryGetComponent(out rigBody);
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

	public void updateSimulationFlag(){
		var desiredFlag = !ragdoll.simulate;
		if (rigBody.isKinematic != desiredFlag)
			rigBody.isKinematic = desiredFlag;		
	}

	void LateUpdate(){
		if (!ragdoll || !rigBody)
			return;
		
		if (ragdoll.simulate){
			if (rigBody.isKinematic)
				rigBody.isKinematic = false;
			targetBone.rotation = transform.rotation;
			targetBone.position = transform.position;
		}
		else{
			if (!rigBody.isKinematic)
				rigBody.isKinematic = true;
			transform.position = targetBone.position;
			transform.rotation = targetBone.rotation;
		}
	}
}
