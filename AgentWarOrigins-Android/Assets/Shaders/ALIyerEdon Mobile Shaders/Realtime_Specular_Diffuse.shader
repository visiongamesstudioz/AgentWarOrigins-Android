// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mobile/ALIyerEdon/Realtime Specular Diffuse"
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[HideInInspector]_SpecColor("SpecularColor",Color)=(1,1,1,1)
		_Smoothness("Smoothness", Range( 0 , 1)) = 1.54
		_Gloss("Gloss", Range( 0 , 1)) = 1.54
		_MainTex("MainTex", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#pragma target 2.0
		#pragma surface surf BlinnPhong keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Smoothness;
		uniform float _Gloss;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float4 tex2DNode1 = tex2D( _MainTex, uv_MainTex );
			o.Albedo = tex2DNode1.rgb;
			o.Specular = ( tex2DNode1.a * _Smoothness );
			o.Gloss = ( tex2DNode1.a * _Gloss );
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=13101
7;29;1010;644;1603.518;279.5048;1.765262;True;False
Node;AmplifyShaderEditor.RangedFloatNode;4;-828.1992,116.3084;Float;False;Property;_Smoothness;Smoothness;0;0;1.54;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;5;-829.8942,242.455;Float;False;Property;_Gloss;Gloss;1;0;1.54;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-831.3662,-121.7042;Float;True;Property;_MainTex;MainTex;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-364,79;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-367.2689,210.4117;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;0,0;Float;False;True;0;Float;ASEMaterialInspector;0;0;BlinnPhong;Mobile/ALIyerEdon/Realtime Specular Diffuse;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;1;4
WireConnection;2;1;4;0
WireConnection;8;0;1;4
WireConnection;8;1;5;0
WireConnection;0;0;1;0
WireConnection;0;3;2;0
WireConnection;0;4;8;0
ASEEND*/
//CHKSM=8CCC19DDE3B4DC9D2DDC7A017D6DD3A965F36A33