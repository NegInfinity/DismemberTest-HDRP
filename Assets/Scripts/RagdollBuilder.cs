using UnityEngine;
using System.Collections.Generic;

public class RagdollBuilder: MonoBehaviour{
	[SerializeField]bool showLimbGizmo = true;
	[SerializeField]bool showSkin = true;
	[SerializeField]bool showBoneBounds = true;

	void drawCross(Vector3 pos, float size){
		var f = size*0.5f;
		var dx = new Vector3(f, 0.0f, 0.0f);
		var dy = new Vector3(0.0f, f, 0.0f);
		var dz = new Vector3(0.0f, 0.0f, f);
		Gizmos.DrawLine(pos - dx, pos + dx);
		Gizmos.DrawLine(pos - dy, pos + dy);
		Gizmos.DrawLine(pos - dz, pos + dz);
	}

	void drawCross(Vector3 pos, Quaternion rot, float size){
		var f = size*0.5f;
		var dx = rot * new Vector3(f, 0.0f, 0.0f);
		var dy = rot * new Vector3(0.0f, f, 0.0f);
		var dz = rot * new Vector3(0.0f, 0.0f, f);
		drawCross(pos, dx, dy, dz);
	}

	void drawCross(Vector3 pos, Vector3 dx, Vector3 dy, Vector3 dz){
		Gizmos.DrawLine(pos - dx, pos + dx);
		Gizmos.DrawLine(pos - dy, pos + dy);
		Gizmos.DrawLine(pos - dz, pos + dz);
	}

	void drawCross(Transform t, float size = 0.25f){
		if (!t)
			return;
		drawCross(t.position, t.rotation, size);
	}

	void drawCrosses(params Transform[] t){
		foreach(var cur in t){
			drawCross(cur);
		}
	}

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
		var anim = GetComponent<Animator>();
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

	void drawBoneBoundsGizmo(SkinnedMeshRenderer[] skelRend){
		var boneBounds = new Dictionary<Transform, ObbData>();
		foreach(var rend in skelRend){
			var mesh = rend.sharedMesh;
			var verts = mesh.vertices;
			var weights = mesh.boneWeights;
			var bones = rend.bones;
			var numBones = mesh.bindposes.Length;
			var matrices = new Matrix4x4[mesh.bindposes.Length];
			//Debug.Log($"numBones: {numBones}");
			for(int i = 0; i < numBones; i++){
				matrices[i] = rend.bones[i].localToWorldMatrix * mesh.bindposes[i];
			}

			var numVerts = verts.Length;
			for(int i = 0; i < numVerts; i++){
				var v = verts[i];
				var w = weights[i];
				var p = skinTransform(v, w, mesh, rend);
				//drawCross(p, 0.02f);
				updateBoneBounds(boneBounds, p, w, bones);
			}
		}

		foreach(var cur in boneBounds.Values){
			//Debug.Log($"bone: {cur.bone}; min: {cur.min}; max: {cur.max}");
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

	void Start(){

	}
}