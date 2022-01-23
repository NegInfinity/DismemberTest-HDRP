using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MeshWorldUvPostprocessor: AssetPostprocessor{
	bool isSupportedAsset(){
		return assetPath.Contains("worldUv", System.StringComparison.OrdinalIgnoreCase);
	}
	void OnPreprocessModel(){
		if (!isSupportedAsset())
			return;
		Debug.Log("creating world uv");
		var importer = assetImporter as ModelImporter;
	}

	void OnPostprocessModel(GameObject obj){
		Debug.Log($"postprocessing before check: {obj.name}");
		if (!isSupportedAsset())
			return;
		Debug.Log($"postprocessing {obj.name}");

		var skinMeshes = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
		Debug.Log($"meshes({skinMeshes.Length});");		

		var meshes = new HashSet<Mesh>();
		foreach(var cur in skinMeshes){
			meshes.Add(cur.sharedMesh);
		}

		bool blenderFix = true;
		
		foreach(var mesh in meshes){
			var numVerts = mesh.vertexCount;			
			var uv3 = new Vector2[numVerts];
			var uv4 = new Vector2[numVerts];
			var verts = mesh.vertices;			
			for(int i = 0; i < mesh.vertexCount; i++){
				var v = verts[i];
				if (blenderFix){
					v = v * 100.0f;
					v = new Vector3(v.x, v.z, -v.y);
				}
				uv3[i] = new Vector2(v.x, v.y);
				uv4[i] = new Vector2(v.z, 1.0f);
			}
			mesh.uv3 = uv3;
			mesh.uv4 = uv4;
		}
	}
}
