using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoInputController : MonoBehaviour{
	[SerializeField] float raycastMaxDistance = 100.0f;
	[SerializeField] float woundRadius = 0.5f;
	[SerializeField] float woundDepth = 0.2f;
	[SerializeField] float impactImpulse = 100.0f;
	// Start is called before the first frame update
	void Start(){
		
	}

	// Update is called once per frame
	void Update(){
		var pos = Input.mousePosition;
		var ray = Camera.main.ScreenPointToRay(pos);

		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, raycastMaxDistance)){
			Debug.DrawLine(ray.origin, hit.point);
		}
		else{
			Debug.DrawLine(ray.origin, ray.origin + ray.direction * raycastMaxDistance);
			return;
		}

		if (!Input.GetMouseButtonDown(0))
			return;

		Debug.Log("Raycast hit");

		var woundChar = hit.collider.GetComponentInParent<WoundCharacter>();
		if (!woundChar){
			Debug.Log("Wound character not found");
			return;
		}
		woundChar.applyWound(hit.collider.gameObject, hit.point, -ray.direction, woundRadius, woundDepth);

		var ragdoll = woundChar.GetComponentInChildren<Ragdoll>();
		if (ragdoll){
			ragdoll.simulate = true;
			ragdoll.updateSimulationFlag();
		}
		var rigBody = hit.collider.GetComponent<Rigidbody>();
		if (rigBody){
			Debug.Log($"Adding force to: {rigBody}");
			rigBody.AddForceAtPosition(ray.direction * impactImpulse, hit.point, ForceMode.Impulse);
		}
	}
}
