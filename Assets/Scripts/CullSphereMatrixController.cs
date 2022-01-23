using UnityEngine;

[ExecuteInEditMode]
public class CullSphereMatrixController: MonoBehaviour{
	[SerializeField] Transform clipSphere = null;
	[SerializeField] float refRadius = 1.0f;
	[SerializeField] string matrixName = "clipSphereMatrix";
	[SerializeField] bool useMaterial = false;

	Renderer rend;
	MaterialPropertyBlock propBlock;

	void OnEnable(){
		propBlock = new();
		rend = GetComponent<Renderer>();
	}

	void Update(){
		if (!rend)
			return;
		if (useMaterial){
			var mat = rend.sharedMaterial;
			if (clipSphere){
				var matrix = clipSphere.transform.worldToLocalMatrix;
				Debug.Log($"Clip sphere matrix:\n{matrix};");
				mat.SetMatrix(matrixName, matrix);
			}
		}
		else{
			rend.GetPropertyBlock(propBlock);
			if (clipSphere){
				var mat = clipSphere.transform.worldToLocalMatrix;
				//Debug.Log($"Clip sphere matrix:\n{mat};");
				propBlock.SetMatrix(matrixName, mat);
			}
			rend.SetPropertyBlock(propBlock);
		}
	}
}