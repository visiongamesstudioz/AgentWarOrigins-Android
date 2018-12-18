Shader "Custom/SurfaceShader_VC_Mobile" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}

	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }

		Tags{ "ForceNoShadowCasting" = "True" }

		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, 
#pragma surface surf Standard  alpha:fade

		// Use shader model 2.0 target, for all mobile
#pragma target 2.0

	sampler2D _MainTex;

	struct Input {
		float2 uv_MainTex;
		float4 vertex : SV_POSITION;
		float4 color : COLOR;
	};

	//void vert(inout appdata_full v, out Input o)
	//{
	//	UNITY_INITIALIZE_OUTPUT(Input, o);
	//	o.color = v.color;
	//}

	fixed4 _Color;

	void surf(Input IN, inout SurfaceOutputStandard o) {
		// Albedo comes from a texture tinted by color
		fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
		o.Albedo = c.rgb*IN.color;
		o.Alpha = c.a*IN.color.a;
	}
	ENDCG
	}
		FallBack "Diffuse"
}