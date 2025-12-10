Shader "Custom/FeatureEdgeWireframe_AlwaysVisible"
{
    Properties
    {
        _Color ("Front Edge Color", Color) = (0, 1, 1, 1)
        _BackColor ("Back Edge Color", Color) = (0, 1, 1, 0.2)
        _Threshold ("Edge Angle Threshold", Range(0,180)) = 30
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" }

        Pass
        {
            Cull Off
            ZWrite Off
            ZTest Always // ← draw regardless of depth
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma geometry geom
            #pragma fragment frag
            #pragma target 4.0
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2g
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct g2f
            {
                float4 pos : SV_POSITION;
                float visibility : TEXCOORD0;
            };

            float4 _Color;
            float4 _BackColor;
            float _Threshold;

            v2g vert(appdata v)
            {
                v2g o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            [maxvertexcount(6)]
            void geom(triangle v2g input[3], inout LineStream<g2f> outputStream)
            {
                // Compute face normal
                float3 N = normalize(cross(input[1].worldPos - input[0].worldPos,
                                           input[2].worldPos - input[0].worldPos));

                // Camera direction
                float3 viewDir = normalize(_WorldSpaceCameraPos - input[0].worldPos);
                float facing = dot(N, viewDir);

                // Emit ALL edges (front + back)
                for (int i = 0; i < 3; i++)
                {
                    g2f o1, o2;
                    o1.pos = input[i].pos;
                    o2.pos = input[(i + 1) % 3].pos;

                    // Visibility: 1 for front, 0 for back
                    o1.visibility = facing > 0 ? 1.0 : 0.0;
                    o2.visibility = o1.visibility;

                    outputStream.Append(o1);
                    outputStream.Append(o2);
                    outputStream.RestartStrip();
                }
            }

            fixed4 frag(g2f i) : SV_Target
            {
                // Use front or back color based on visibility
                return lerp(_BackColor, _Color, step(0.5, i.visibility));
            }
            ENDCG
        }
    }
}
