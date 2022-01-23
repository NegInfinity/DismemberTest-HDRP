using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StaticRecolor: MonoBehaviour{
	// Start is called before the first frame update
	[SerializeField] string paramName = "_Color";
	[SerializeField] Color _color = Color.white;
	
	Color lastColor = Color.white;
	public Color color{
		get => _color;
		set {
			if (_color == value)
				return;
			_color = value;
			onValueChanged();
		}
	}

	MaterialPropertyBlock propBlock;
	private Renderer rend;

	void OnEnable(){
		propBlock = new();
		rend = GetComponent<Renderer>();
		onValueChanged();
	}

	void onValueChanged(){
		if (!rend)
			return;
		rend.GetPropertyBlock(propBlock);
		propBlock.SetColor(paramName, _color);
		rend.SetPropertyBlock(propBlock);
		lastColor = _color;
	}

	void Update(){
		if (lastColor == _color)
			return;
		onValueChanged();
	}
}
