using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RagdollBuilder))]
public class RagdollBuilderEditor: Editor{
	public override void OnInspectorGUI(){
		base.OnInspectorGUI();

		var obj = target as RagdollBuilder;
		if (!obj)
			return;

		GUILayout.BeginHorizontal();
		bool ragdollRequested = GUILayout.Button("Build ragdoll");
		GUILayout.EndHorizontal();
		if (!ragdollRequested)
			return;
		
		buildRagdoll(obj);
	}

	void buildRagdoll(RagdollBuilder rb){
		if (!rb)
			return;
		rb.buildRagdoll();
	}
}