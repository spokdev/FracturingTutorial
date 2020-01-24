Shader "Custom/FractureSurfaceShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        //---Properties for fracture displacement
        _FractureAmount("Fracture Amount", Range(0,1)) = 0.0
        _TranslateAmount("Translate Amount", float) = 10
        _RotateXAmount("_RotateXAmount", float) = 1
        _RotateYAmount("_RotateYAmount", float) = 1
        _RotateZAmount("_RotateZAmount", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        //---Need to add vertex:vert for custom vertex program
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

    //---Custom vertex code
        float _FractureAmount;
        float _TranslateAmount;
        float _RotateXAmount;
        float _RotateYAmount;
        float _RotateZAmount;

        void vert(inout appdata_full v)
        {
            float4 localVertexPos = v.vertex;
            float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
            //transform shard position from model space to world space
            float3 fracturedPieceOffsetWorldSpace = mul(unity_ObjectToWorld, float4(v.texcoord3.xyz, 1.0));
            float3 fracturedPieceDirFromCenter = normalize(fracturedPieceOffsetWorldSpace.xyz - objectOrigin.xyz);
            float4 worldVertexPos = mul(unity_ObjectToWorld, localVertexPos);
            float4 worldVertexPosRelative = worldVertexPos - (float4(fracturedPieceOffsetWorldSpace.xyz, 0));

            float3 translation = _TranslateAmount * _FractureAmount * fracturedPieceDirFromCenter;


            float4x4 translateMatrix = float4x4(1, 0, 0, translation.x,
                                                0, 1, 0, translation.y,
                                                0, 0, 1, translation.z,
                                                0, 0, 0, 1);

            float _sX = 1 - _FractureAmount;
            float _sY = _sX;
            float _sZ = _sX;

            float4x4 scaleMatrix = float4x4(_sX, 0, 0, 0,
                                              0, _sY, 0, 0,
                                              0, 0, _sZ, 0,
                                              0, 0, 0, 1);

            float angleX = radians(_RotateXAmount * _FractureAmount);
            float c = cos(angleX);
            float s = sin(angleX);
            float4x4 rotateXMatrix = float4x4(1, 0, 0, 0,
                                              0, c, -s, 0,
                                              0, s, c, 0,
                                              0, 0, 0, 1);

            float angleY = radians(_RotateYAmount * _FractureAmount);
            c = cos(angleY);
            s = sin(angleY);
            float4x4 rotateYMatrix = float4x4(c, 0, s, 0,
                                              0, 1, 0, 0,
                                             -s, 0, c, 0,
                                              0, 0, 0, 1);

            float angleZ = radians(_RotateZAmount * _FractureAmount);
            c = cos(angleZ);
            s = sin(angleZ);
            float4x4 rotateZMatrix = float4x4(c, -s, 0, 0,
                                               s, c, 0, 0,
                                               0, 0, 1, 0,
                                               0, 0, 0, 1);

            //scale - rotate - translate
            float4 localScaledTranslated = mul(worldVertexPosRelative, scaleMatrix);
            float4 localScaledTranslatedRotX = mul(localScaledTranslated, rotateXMatrix);
            float4 localScaledTranslatedRotXY = mul(localScaledTranslatedRotX, rotateYMatrix);
            float4 localScaledTranslatedRotXYZ = mul(localScaledTranslatedRotXY, rotateZMatrix);
            localScaledTranslatedRotXYZ = mul(translateMatrix, localScaledTranslatedRotXYZ);

            localScaledTranslatedRotXYZ += (float4(fracturedPieceOffsetWorldSpace.xyz, 0));
            localScaledTranslatedRotXYZ = mul(unity_WorldToObject, localScaledTranslatedRotXYZ);

            v.vertex = localScaledTranslatedRotXYZ;

            float3 normal = v.normal;
            normal = mul(normal, rotateXMatrix);
            normal = mul(normal, rotateYMatrix);
            normal = mul(normal, rotateZMatrix);
            v.normal = normal;
        }

//----

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
