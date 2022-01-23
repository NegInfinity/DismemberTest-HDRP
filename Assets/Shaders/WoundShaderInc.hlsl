#ifndef WOUNDSHADERINC_H
#define WOUNDSHADERINC_H

void sampleWound_float(float3 pos, float4 inColor, out float4 col, out float alpha){
	float3 spherePos = mul(clipSphereMatrix, float4(pos, 1.0)).xyz;
	float sphereBlend = clamp(abs(spherePos.z * 2.0) - 1.0, 0.0, 1.0);
	float2 bloodCoord = (spherePos.xy * _BloodScale) * 0.5 + 0.5;	
	//float4 bloodColor = tex2D(_BloodTex, bloodCoord);
	float4 bloodColor = SAMPLE_TEXTURE2D(_BloodTex, SamplerState_Linear_Clamp, bloodCoord);
	bloodColor = lerp(bloodColor, 1.0, sphereBlend);
	col = bloodColor * inColor;
	alpha = col.w;
} 

void vec3toPos_float(float3 pos, out float4 res){
	res = float4(pos, 1.0);
}

void uvsToPos_float(float2 uv1, float2 uv2, out float4 res){
	res = float4(uv1, uv2);
}

void debugPosToColor_float(float4 inPos, out float4 res){
	res.xyz = frac(inPos.xyz * 10.0f);
	res.w = inPos.w;
}
#endif