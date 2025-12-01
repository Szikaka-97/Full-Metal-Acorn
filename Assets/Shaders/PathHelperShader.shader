Shader "Custom/PathHelperShader"
{
    Properties
    {
        [MainColor] _BaseColor("Base Color", Color) = (1, 1, 1, 1)
        [MainTexture] _BaseMap("Base Map", 2D) = "white"
		_Rotation("Path Rotation", int) = 0
		_Direction("Path Direction", int) = 0
		_Progress("Path Progress", float) = 0
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            CBUFFER_START(UnityPerMaterial)
                half4 _BaseColor;
                float4 _BaseMap_ST;
				float _Direction;
				float _Rotation;
				float _Progress;
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                
				IN.uv.y += 1.0f;
				IN.uv.x += _Rotation;
				IN.uv *= 0.5f;

				OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
				if (_Progress > 1) {
					discard;
				}

				float uvX = (IN.uv.x - (0.5f * _Rotation) - 7.0f/64.0f) / (18.0f/64.0f);

				if (_Direction) {
					uvX = 1.0f - uvX;
				}

				if (uvX < _Progress) {
					discard;
				}

                half4 color = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv) * _BaseColor;

				if (color.a == 0) {
					discard;
				}

                return color;
            }
            ENDHLSL
        }
    }
}
