using UnityEngine;
using System.Collections.Generic;
using System.Linq;

using static GizmoTools;

public class RagdollBuilder: MonoBehaviour{
	[SerializeField]bool showLimbGizmo = true;
	[SerializeField]bool showSkin = true;
	[SerializeField]bool showBoneBounds = true;
	[SerializeField]float density = 1000.0f;

	void drawLimb(params Transform[] t){
		for(int i = 0; i < t.Length; i++){
			drawCross(t[i]);
		}
		for(int i = 1; i < t.Length; i++){
			var prev = t[i-1];
			var cur = t[i];
			if (prev && cur)
				Gizmos.DrawLine(prev.position, cur.position);
		}
	}

	Vector3 skinBoneTransform(Vector3 p, int boneIndex, Mesh mesh, SkinnedMeshRenderer rend){
		return (rend.bones[boneIndex].localToWorldMatrix * mesh.bindposes[boneIndex]).MultiplyPoint(p);
	}

	Vector3 skinTransform(Vector3 p, BoneWeight weight, Mesh mesh, SkinnedMeshRenderer rend){
		var result = Vector3.zero;		

		result += skinBoneTransform(p, weight.boneIndex0, mesh, rend) * weight.weight0;
		result += skinBoneTransform(p, weight.boneIndex1, mesh, rend) * weight.weight1;
		result += skinBoneTransform(p, weight.boneIndex2, mesh, rend) * weight.weight2;
		result += skinBoneTransform(p, weight.boneIndex3, mesh, rend) * weight.weight3;

		return result;
	}

	Vector3 skinTransform(Vector3 p, BoneWeight weight, Matrix4x4[] matrices){
		var result = Vector3.zero;		

		result += matrices[weight.boneIndex0].MultiplyPoint(p) * weight.weight0;
		result += matrices[weight.boneIndex1].MultiplyPoint(p) * weight.weight1;
		result += matrices[weight.boneIndex2].MultiplyPoint(p) * weight.weight2;
		result += matrices[weight.boneIndex3].MultiplyPoint(p) * weight.weight3;

		return result;
	}

	public class ObbData{
		public Transform bone = null;
		public Vector3 min = Vector3.zero;
		public Vector3 max = Vector3.zero;
		public void update(Vector3 pos){
			var p = bone.InverseTransformVector(pos - bone.position);
			min = Vector3.Min(min, p);
			max = Vector3.Max(max, p);
		}

		public ObbData(Transform bone_, Vector3 firstPoint){
			bone = bone_;
			min = max = bone.InverseTransformVector(firstPoint - bone.position);
		}

		public void drawGizmo(){
			var x1 = new Vector3(min.x, 0.0f, 0.0f);
			var x2 = new Vector3(max.x, 0.0f, 0.0f);
			var y1 = new Vector3(0.0f, min.y, 0.0f);
			var y2 = new Vector3(0.0f, max.y, 0.0f);
			var z1 = new Vector3(0.0f, 0.0f, min.z);
			var z2 = new Vector3(0.0f, 0.0f, max.z);

			x1 = bone.TransformVector(x1);
			x2 = bone.TransformVector(x2);
			y1 = bone.TransformVector(y1);
			y2 = bone.TransformVector(y2);
			z1 = bone.TransformVector(z1);
			z2 = bone.TransformVector(z2);
			var p = bone.position;

			Gizmos.DrawLine(p + x1 + y1 + z1, p + x2 + y1 + z1);
			Gizmos.DrawLine(p + x1 + y2 + z1, p + x2 + y2 + z1);
			Gizmos.DrawLine(p + x1 + y1 + z2, p + x2 + y1 + z2);
			Gizmos.DrawLine(p + x1 + y2 + z2, p + x2 + y2 + z2);

			Gizmos.DrawLine(p + x1 + y1 + z1, p + x1 + y2 + z1);
			Gizmos.DrawLine(p + x2 + y1 + z1, p + x2 + y2 + z1);
			Gizmos.DrawLine(p + x1 + y1 + z2, p + x1 + y2 + z2);
			Gizmos.DrawLine(p + x2 + y1 + z2, p + x2 + y2 + z2);

			Gizmos.DrawLine(p + x1 + y1 + z1, p + x1 + y1 + z2);
			Gizmos.DrawLine(p + x2 + y1 + z1, p + x2 + y1 + z2);
			Gizmos.DrawLine(p + x1 + y2 + z1, p + x1 + y2 + z2);
			Gizmos.DrawLine(p + x2 + y2 + z1, p + x2 + y2 + z2);
		}
	}

	void drawSkeletonGizmo(SkinnedMeshRenderer[] skelRend){
		foreach(var rend in skelRend){
			var mesh = rend.sharedMesh;

			var trigs = mesh.triangles;
			var verts = mesh.vertices;
			var weights = mesh.boneWeights;
			var numBones = mesh.bindposes.Length;
			var matrices = new Matrix4x4[mesh.bindposes.Length];
			for(int i = 0; i < numBones; i++){
				matrices[i] = rend.bones[i].localToWorldMatrix * mesh.bindposes[i];
			}
			for(int i = 0; i < trigs.Length; i += 3){
				var aIdx = trigs[i+0];
				var bIdx = trigs[i+1];
				var cIdx = trigs[i+2];

				var a = verts[aIdx];
				var b = verts[bIdx];
				var c = verts[cIdx];

				var aWeights = weights[aIdx];
				var bWeights = weights[bIdx];
				var cWeights = weights[cIdx];

				var a1 = skinTransform(a, aWeights, matrices);
				var b1 = skinTransform(b, aWeights, matrices);
				var c1 = skinTransform(c, aWeights, matrices);

				Gizmos.DrawLine(a1, b1);
				Gizmos.DrawLine(b1, c1);
				Gizmos.DrawLine(a1, c1);
			}
		}
	}

	void drawLimbGizmo(){
		var anim = GetComponentInChildren<Animator>();
		if (!anim)
			return;

		var hip = anim.GetBoneTransform(HumanBodyBones.Hips);
		drawCross(hip);

		var lUpperLeg = anim.GetBoneTransform(HumanBodyBones.LeftUpperLeg);
		var rUpperLeg = anim.GetBoneTransform(HumanBodyBones.RightUpperLeg);
		var lLowerLeg = anim.GetBoneTransform(HumanBodyBones.LeftLowerLeg);
		var rLowerLeg = anim.GetBoneTransform(HumanBodyBones.RightLowerLeg);
		var lFoot = anim.GetBoneTransform(HumanBodyBones.LeftFoot);
		var rFoot = anim.GetBoneTransform(HumanBodyBones.RightFoot);
		drawLimb(lUpperLeg, lLowerLeg, lFoot);
		drawLimb(rUpperLeg, rLowerLeg, rFoot);

		var lUpperArm = anim.GetBoneTransform(HumanBodyBones.LeftUpperArm);
		var rUpperArm = anim.GetBoneTransform(HumanBodyBones.RightUpperArm);
		var lLowerArm = anim.GetBoneTransform(HumanBodyBones.LeftLowerArm);
		var rLowerArm = anim.GetBoneTransform(HumanBodyBones.RightLowerArm);
		var lHand = anim.GetBoneTransform(HumanBodyBones.LeftHand);
		var rHand = anim.GetBoneTransform(HumanBodyBones.RightHand);
		drawLimb(lUpperArm, lLowerArm, lHand);
		drawLimb(rUpperArm, rLowerArm, rHand);

		var spine = anim.GetBoneTransform(HumanBodyBones.Spine);
		var chest = anim.GetBoneTransform(HumanBodyBones.Chest);
		var neck = anim.GetBoneTransform(HumanBodyBones.Neck);
		var head = anim.GetBoneTransform(HumanBodyBones.Head);
		drawLimb(spine, chest, neck, head);
	}

	Dictionary<Transform,ObbData> buildBoneBounds(){
		var skelRend = GetComponentsInChildren<SkinnedMeshRenderer>();
		return buildBoneBounds(skelRend);
	}

	Dictionary<Transform,ObbData> buildBoneBounds(SkinnedMeshRenderer[] skelRend){
		var result = new Dictionary<Transform, ObbData>();
		foreach(var rend in skelRend){
			var mesh = rend.sharedMesh;
			var verts = mesh.vertices;
			var weights = mesh.boneWeights;
			var bones = rend.bones;
			var numBones = mesh.bindposes.Length;
			var matrices = new Matrix4x4[mesh.bindposes.Length];
			for(int i = 0; i < numBones; i++){
				matrices[i] = rend.bones[i].localToWorldMatrix * mesh.bindposes[i];
			}

			var numVerts = verts.Length;
			for(int i = 0; i < numVerts; i++){
				var v = verts[i];
				var w = weights[i];
				var p = skinTransform(v, w, mesh, rend);
				updateBoneBounds(result, p, w, bones);
			}
		}
		return result;
	}

	void drawBoneBoundsGizmo(SkinnedMeshRenderer[] skelRend){
		var boneBounds = buildBoneBounds();

		foreach(var cur in boneBounds.Values){
			cur.drawGizmo();
		}
	}

	void drawGizmosBody(){
		if (!(showLimbGizmo || showSkin || showBoneBounds))
			return;

		var skelRend = GetComponentsInChildren<SkinnedMeshRenderer>();
		if (showSkin){
			drawSkeletonGizmo(skelRend);
		}

		if (showLimbGizmo)
			drawLimbGizmo();

		if (showBoneBounds){
			drawBoneBoundsGizmo(skelRend);
		}
	}

	static void updateBoneBounds(Dictionary<Transform, ObbData> bounds, Vector3 v, int boneIndex, float weight, Transform[] bones){
		if (weight <= 0.0f)
			return;
		ObbData obbData;
		var bone = bones[boneIndex];
		if (!bounds.TryGetValue(bone, out obbData)){
			obbData = new ObbData(bone, v);
			bounds.Add(bone, obbData);
		}
		else{
			obbData.update(v);
		}
	}

	static void updateBoneBounds(Dictionary<Transform, ObbData> bounds, Vector3 v, BoneWeight w, Transform[] bones){
		updateBoneBounds(bounds, v, w.boneIndex0, w.weight0, bones);
		updateBoneBounds(bounds, v, w.boneIndex1, w.weight1, bones);
		updateBoneBounds(bounds, v, w.boneIndex2, w.weight2, bones);
		updateBoneBounds(bounds, v, w.boneIndex3, w.weight3, bones);
	}

	void drawGizmos(Color c){
		var oldColor= Gizmos.color;
		Gizmos.color = c;
		drawGizmosBody();

		Gizmos.color = oldColor;
	}

	void OnDrawGizmos(){
		drawGizmos(Color.yellow);
	}	

	void OnDrawGizmosSelected(){
		drawGizmos(Color.white);
	}	

	static readonly Vector3[] axisCandidates = new Vector3[]{
		Vector3.right, 
		-Vector3.right,
		Vector3.up,
		-Vector3.up,
		Vector3.forward,
		-Vector3.forward
	};

	static float getLocalAxisDot(Transform t, Vector3 local, Vector3 global){
		var v = t.TransformVector(local);
		return Vector3.Dot(v, global);
	}

	static Vector3 findClosestLocalAxis(Transform t, Vector3 globalAxis){
		var result = axisCandidates
			.Select(v => (v, getLocalAxisDot(t, v, globalAxis)))
			.Aggregate((cur, next) => (cur.Item2 > next.Item2) ? cur: next)
			.v;
		return result;
	}

	Dictionary<HumanBodyBones, Transform> humanBoneToTransform = new();
	Dictionary<Transform, RagdollPart> animToRagdoll = new();

	RagdollPart getHumanRagdollPart(HumanBodyBones boneId){
		Transform animBone = null;
		if (!humanBoneToTransform.TryGetValue(boneId, out animBone))
			return null;
		RagdollPart result = null;
		if (!animToRagdoll.TryGetValue(animBone, out result))
			return null;
		return result;
	}

	void linkHumanBones(HumanBodyBones curBoneId, HumanBodyBones parentBoneId, System.Action<Rigidbody, Rigidbody> linkCallback){
		var curPart = getHumanRagdollPart(curBoneId);
		var parentPart = getHumanRagdollPart(parentBoneId);
		if (!curPart || !parentPart)
			return;
		var curRigBody = curPart.GetComponent<Rigidbody>();
		var parentRigBody = parentPart.GetComponent<Rigidbody>();
		if (!curRigBody || !parentRigBody)
			return;
		Debug.Log($"{curRigBody}, {parentRigBody}");

		linkCallback(curRigBody, parentRigBody);
	}

	void linkHingeBones(HumanBodyBones curBoneId, HumanBodyBones parentBoneId, Vector3 globalAxis, float minLimit, float maxLimit){
		linkHumanBones(curBoneId, parentBoneId, 
			(curRigBody, parentRigBody) => {
				var joint = curRigBody.gameObject.AddComponent<HingeJoint>();
				joint.connectedBody = parentRigBody;
				var localAxis = findClosestLocalAxis(curRigBody.transform, globalAxis);
				joint.axis = localAxis;
				joint.anchor = Vector3.zero;
				var limits = joint.limits;
				limits.min = minLimit;
				limits.max = maxLimit;
				joint.useLimits = true;
				joint.limits = limits;
			}
		);
	}

	void linkBallSocketBones(HumanBodyBones curBoneId, HumanBodyBones parentBoneId, 
			Vector3 twistAxis, Vector3 swingAxis, System.Action<CharacterJoint> configCallback = null){
		linkHumanBones(curBoneId, parentBoneId, 
			(curRigBody, parentRigBody) => {
				var joint = curRigBody.gameObject.AddComponent<CharacterJoint>();
				joint.connectedBody = parentRigBody;
				var localTwist = findClosestLocalAxis(curRigBody.transform, twistAxis);
				var localSwing = findClosestLocalAxis(curRigBody.transform, swingAxis);
				joint.axis = localTwist;
				joint.swingAxis = localSwing;
				if (configCallback != null)
					configCallback(joint);
			}
		);
	}

	public void buildRagdoll(){
		var boneBounds = buildBoneBounds();
		var ragdoll = new GameObject("ragdoll");

		var humanBodyIds = new HumanBodyBones[]{
			HumanBodyBones.Hips,
			HumanBodyBones.Chest,
			HumanBodyBones.UpperChest,
			HumanBodyBones.Spine,
			HumanBodyBones.Neck,
			HumanBodyBones.Head,
			HumanBodyBones.LeftUpperArm,
			HumanBodyBones.LeftLowerArm,
			HumanBodyBones.LeftHand,
			HumanBodyBones.RightUpperArm,
			HumanBodyBones.RightLowerArm,
			HumanBodyBones.RightHand,
			HumanBodyBones.LeftUpperLeg,
			HumanBodyBones.LeftLowerLeg,
			HumanBodyBones.LeftFoot,
			HumanBodyBones.RightUpperLeg,
			HumanBodyBones.RightLowerLeg,
			HumanBodyBones.RightFoot
		};

		var skinRend = GetComponentsInChildren<SkinnedMeshRenderer>();

		var anim = GetComponentInChildren<Animator>();

		List<Transform> humanBones = new();
		humanBoneToTransform.Clear();
		if (anim){
			foreach(var curId in humanBodyIds){
				var animBone = anim.GetBoneTransform(curId);
				if (!animBone)
					continue;
				humanBones.Add(animBone);
				humanBoneToTransform.Add(curId, animBone);
			}
			//humanBones = humanBodyIds.Select(b => anim.GetBoneTransform(b)).Where(b => b != null).ToList();
		}

		ragdoll.transform.SetParent(transform);
		ragdoll.transform.localPosition = Vector3.zero;
		ragdoll.transform.localRotation = Quaternion.identity;
		ragdoll.transform.localScale = Vector3.one;
		ragdoll.AddComponent<Ragdoll>();

		var filteredBounds = boneBounds.Where(b => (humanBones.Count == 0) || (humanBones.Contains(b.Value.bone)));
		animToRagdoll.Clear();
		foreach(var i in filteredBounds){
			var bone = i.Key;
			var curBound = i.Value;
			var obj = new GameObject(bone.name);
			obj.transform.SetParent(ragdoll.transform);
			obj.transform.rotation = bone.rotation;
			obj.transform.position = bone.position;

			var center = (curBound.min + curBound.max)*0.5f;
			var size = curBound.max - curBound.min;
			
			var scale = new Vector3(
				curBound.bone.TransformVector(new Vector3(1.0f, 0.0f, 0.0f)).magnitude,
				curBound.bone.TransformVector(new Vector3(0.0f, 1.0f, 0.0f)).magnitude,
				curBound.bone.TransformVector(new Vector3(0.0f, 0.0f, 1.0f)).magnitude
			);

			center.x *= scale.x;
			center.y *= scale.y;
			center.z *= scale.z;
			size.x *= scale.x;
			size.y *= scale.y;
			size.z *= scale.z;

			var volume = size.x * size.y * size.z;
			var mass = volume * density;

			var boxCollider= obj.AddComponent<BoxCollider>();

			boxCollider.center = center;
			boxCollider.size = size;

			var visualizer = obj.AddComponent<ColliderVisualizer>();

			var ragdollPart = obj.AddComponent<RagdollPart>();
			ragdollPart.targetBone = bone;
			ragdollPart.colliderBoxSize = size;
			ragdollPart.colliderBoxCenter = center;
			ragdollPart.originalPose.position = bone.transform.position;
			ragdollPart.originalPose.rotation = bone.transform.rotation;

			var rigBody = obj.AddComponent<Rigidbody>();
			rigBody.mass = mass;
			rigBody.interpolation = RigidbodyInterpolation.Interpolate;

			animToRagdoll.Add(bone, ragdollPart);
		}

		var elbowLimits = new JointLimits();
		elbowLimits.min = 0.0f;
		elbowLimits.max = 90.0f;

		System.Action<CharacterJoint> hipConfig = (joint) => {
		};

		linkHingeBones(HumanBodyBones.LeftLowerLeg, HumanBodyBones.LeftUpperLeg, -Vector3.right, -90.0f, 0.0f);
		linkHingeBones(HumanBodyBones.RightLowerLeg, HumanBodyBones.RightUpperLeg, -Vector3.right, -90.0f, 0.0f);
		linkHingeBones(HumanBodyBones.LeftLowerArm, HumanBodyBones.LeftUpperArm, Vector3.up, 0.0f, 90.0f);
		linkHingeBones(HumanBodyBones.RightLowerArm, HumanBodyBones.RightUpperArm, -Vector3.up, 0.0f, 90.0f);

		linkBallSocketBones(HumanBodyBones.Head, HumanBodyBones.Neck, Vector3.up, -Vector3.right);
		linkBallSocketBones(HumanBodyBones.Neck, HumanBodyBones.Chest, Vector3.up, -Vector3.right);
		linkBallSocketBones(HumanBodyBones.Chest, HumanBodyBones.Spine, Vector3.up, -Vector3.right);
		linkBallSocketBones(HumanBodyBones.Spine, HumanBodyBones.Hips, Vector3.up, -Vector3.right);

		linkBallSocketBones(HumanBodyBones.LeftHand, HumanBodyBones.LeftLowerArm, -Vector3.right, Vector3.forward);
		linkBallSocketBones(HumanBodyBones.RightHand, HumanBodyBones.RightLowerArm, Vector3.right, Vector3.forward);
		linkBallSocketBones(HumanBodyBones.LeftUpperArm, HumanBodyBones.Chest, -Vector3.right, Vector3.forward);
		linkBallSocketBones(HumanBodyBones.RightUpperArm, HumanBodyBones.Chest, Vector3.right, Vector3.forward);

		linkBallSocketBones(HumanBodyBones.LeftFoot, HumanBodyBones.LeftLowerLeg, -Vector3.up, -Vector3.right);
		linkBallSocketBones(HumanBodyBones.RightFoot, HumanBodyBones.RightLowerLeg, -Vector3.up,-Vector3.right);
		linkBallSocketBones(HumanBodyBones.LeftUpperLeg, HumanBodyBones.Hips, -Vector3.up, -Vector3.right);
		linkBallSocketBones(HumanBodyBones.RightUpperLeg, HumanBodyBones.Hips, -Vector3.up, Vector3.right);
	}

	void Start(){
	}
}