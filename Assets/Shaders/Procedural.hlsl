#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
	StructuredBuffer<float4> positionBuffer;
#endif

void setup() {
#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
	float4 data = positionBuffer[unity_InstanceID];
	float rot = data.w;
	float size = 1;
	float rotation = data.w * data.w * _Time.y * 0;

	unity_ObjectToWorld._11_21_31_41 = float4(size, 0, 0, 0);
	unity_ObjectToWorld._12_22_32_42 = float4(0, size, 0, 0);
	unity_ObjectToWorld._13_23_33_43 = float4(0, 0, size, 0);
	unity_ObjectToWorld._14_24_34_44 = float4(data.xyz, 1);

	// float s, c;
	// sincos(rot, s, c);
	// float4x4 rotateX = float4x4(
	// 	1, 0, 0, 0,
	// 	0, c, -s, 0,
	// 	0, s, c, 0,
	// 	0, 0, 0, 1
	// );

	// unity_ObjectToWorld = mul(unity_ObjectToWorld, rotateX);

	unity_WorldToObject = unity_ObjectToWorld;
	unity_WorldToObject._14_24_34 *= -1;
	unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
#endif
}

void Empty_float(in float3 In, out float3 Out) {
	Out = In;
}

void Empty_half(in half3 In, out half3 Out) {
	Out = In;
}