Shader "_XXGAME/Other/OutLine"{
    Properties {
        _OutlineWidth("OutlineWidth", float) = 0
        _OutlineColor("OutlineColor", Color) = (0, 0, 0, 0)
    }
    SubShader {
        Tags {"RenderType" = "Opaque" "RenderPipeLine" = "UniversalPipeline"}
        Pass {
            Name "outline"
            Tags {"LightMode" = "UniversalForward"}

            Cull Front
            ZWrite On
            ZTest LEqual
            HLSLPROGRAM

            #pragma target 2.0
            #pragma vertex vert
			#pragma fragment frag
			
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"

            struct Attributes {
                float3 positionOS: POSITION;
                float3 tangentOS: TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings {
                float4 positionCS: SV_POSITION;
            };
            
            CBUFFER_START(UnityPerMaterial)
            float4 _OutlineColor;
            float _OutlineWidth;
            CBUFFER_END
            
            Varyings vert(Attributes IN) {
				Varyings OUT;
 
 
                // 把顶点坐标转换到裁剪空间
                float4 clipPosition =  TransformObjectToHClip(IN.positionOS.xyz);

                // 获取世界空间的法线
                VertexNormalInputs normalInput = GetVertexNormalInputs(IN.tangentOS);
                //法线转换到裁剪空间
                float3 normalCS = TransformWorldToHClipDir(normalInput.normalWS);

                float4 scaledScreenParams = GetScaledScreenParams();
                float ScaleX = abs(scaledScreenParams.x / scaledScreenParams.y);//求得X因屏幕比例缩放的倍数
                float2 extendDis = normalize(normalCS.xy) *(_OutlineWidth*0.01);//根据法线和线宽计算偏移量

                // 根据屏幕比例除以缩放，因为屏幕比例不是1：1的，不然会根据长宽比不同导致上下宽度不同
                extendDis.x /= ScaleX;
                clipPosition.xy += extendDis * clipPosition.w;
                OUT.positionCS = clipPosition;
				return OUT;
			}
            
			half4 frag(Varyings IN) : SV_Target {
				return _OutlineColor;
			}
            ENDHLSL
        }
    }
}