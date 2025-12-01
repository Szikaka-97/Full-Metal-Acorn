#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
	StructuredBuffer<uint> dataBuffer;
#endif

float3 gridPos;
float selected;

void setup() {
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
	const float size = 1;

	const float2x2 basis = float2x2(
		float2(0.5, 0.5),
		float2(-0.25, 0.25)
	);
	const float2x2 inverseBasis = float2x2(
		float2(1, -2),
		float2(1, 2)
	);

	const uint xPos = unity_InstanceID % uint(_GroundSize.x);
	const uint yPos = unity_InstanceID / uint(_GroundSize.x);
	gridPos = float3(xPos, yPos * 0.25, yPos);

	gridPos.x += 0.5 * (yPos % 2u);

	selected = float(dataBuffer[unity_InstanceID]);
	
	gridPos.xy -= float2(_GroundSize.x * 0.5, _GroundSize.y * 0.125);

	unity_ObjectToWorld._11_21_31_41 = float4(size, 0, 0, 0);
	unity_ObjectToWorld._12_22_32_42 = float4(0, size, 0, 0);
	unity_ObjectToWorld._13_23_33_43 = float4(0, 0, size, 0);
	unity_ObjectToWorld._14_24_34_44 = float4(0, 0, 0, 1);
#endif
}

void InstancingParams_float(in float3 In, out float3 Out, out bool isSelected) {
	Out = In + gridPos;
	isSelected = selected > 0;
}

void InstancingParams_float(in half3 In, out half3 Out, out bool isSelected) {
	Out = In + gridPos;
	isSelected = selected > 0;
}