﻿Shader "Custom/TraceShader" {
    SubShader {
        Pass {
			Cull Back
			//Blend One One
			ZTest Always
			ZWrite On
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag


            float4 vert(float4 v:POSITION) : SV_POSITION {
                return mul (UNITY_MATRIX_MVP, v);
            }

            fixed4 frag() : SV_Target {
                return fixed4(1.0,0.0,0.0,1.0);
            }

            ENDCG
        }
    }
}