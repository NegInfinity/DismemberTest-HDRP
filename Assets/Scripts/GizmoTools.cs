using UnityEngine;

public static class GizmoTools{
	public static Vector3 circlePoint(Vector3 pos, Vector3 x, Vector3 y, float t){
		var angle = Mathf.PI * 2.0f * t;
		return pos + x * Mathf.Cos(angle) + y * Mathf.Sin(angle);
	}
	
	public static Vector3 circlePoint(Transform obj, Vector3 pos, Vector3 x, Vector3 y, float t){
		return obj.TransformPoint(circlePoint(pos, x, y, t));
	}

	public static void drawCircle(Transform obj, Vector3 pos,  Vector3 x, Vector3 y, int numSegments){
		var step = 1.0f/numSegments;
		for(int i = 0; i < numSegments; i++){
			var t0 = i*step;
			var t1 = t0 + step;
			var p0 = circlePoint(obj, pos, x, y, t0);
			var p1 = circlePoint(obj, pos, x, y, t1);
			Gizmos.DrawLine(p0, p1);
		}
	}

	public static void drawCross(Vector3 pos, float size){
		var f = size*0.5f;
		var dx = new Vector3(f, 0.0f, 0.0f);
		var dy = new Vector3(0.0f, f, 0.0f);
		var dz = new Vector3(0.0f, 0.0f, f);
		Gizmos.DrawLine(pos - dx, pos + dx);
		Gizmos.DrawLine(pos - dy, pos + dy);
		Gizmos.DrawLine(pos - dz, pos + dz);
	}

	public static void drawCross(Vector3 pos, Quaternion rot, float size){
		var f = size*0.5f;
		var dx = rot * new Vector3(f, 0.0f, 0.0f);
		var dy = rot * new Vector3(0.0f, f, 0.0f);
		var dz = rot * new Vector3(0.0f, 0.0f, f);
		drawCross(pos, dx, dy, dz);
	}

	public static void drawCross(Vector3 pos, Vector3 dx, Vector3 dy, Vector3 dz){
		Gizmos.DrawLine(pos - dx, pos + dx);
		Gizmos.DrawLine(pos - dy, pos + dy);
		Gizmos.DrawLine(pos - dz, pos + dz);
	}

	public static void drawCross(Transform t, float size = 0.25f){
		if (!t)
			return;
		drawCross(t.position, t.rotation, size);
	}

	public static void drawCrosses(params Transform[] t){
		foreach(var cur in t){
			drawCross(cur);
		}
	}

	public static void drawBox(Vector3 pos, Vector3 dx, Vector3 dy, Vector3 dz){
		Gizmos.DrawLine(pos - dx - dy - dz, pos + dx - dy - dz);
		Gizmos.DrawLine(pos - dx + dy - dz, pos + dx + dy - dz);
		Gizmos.DrawLine(pos - dx - dy + dz, pos + dx - dy + dz);
		Gizmos.DrawLine(pos - dx + dy + dz, pos + dx + dy + dz);

		Gizmos.DrawLine(pos - dx - dy - dz, pos - dx + dy - dz);
		Gizmos.DrawLine(pos + dx - dy - dz, pos + dx + dy - dz);
		Gizmos.DrawLine(pos - dx - dy + dz, pos - dx + dy + dz);
		Gizmos.DrawLine(pos + dx - dy + dz, pos + dx + dy + dz);

		Gizmos.DrawLine(pos - dx - dy - dz, pos - dx - dy + dz);
		Gizmos.DrawLine(pos + dx - dy - dz, pos + dx - dy + dz);
		Gizmos.DrawLine(pos - dx + dy - dz, pos - dx + dy + dz);
		Gizmos.DrawLine(pos + dx + dy - dz, pos + dx + dy + dz);
	}

	public static void drawBox(Vector3 pos, Quaternion rot, Vector3 size){
		var dx = rot * new Vector3(size.x * 0.5f, 0.0f, 0.0f);
		var dy = rot * new Vector3(0.0f, size.y * 0.5f, 0.0f);
		var dz = rot * new Vector3(0.0f, 0.0f, size.z * 0.5f);
		drawBox(pos, dx, dy, dz);
	}
}
