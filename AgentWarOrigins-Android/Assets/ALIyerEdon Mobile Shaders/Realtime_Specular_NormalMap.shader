// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Mobile/ALIyerEdon/Realtime Specular + Normal Map "
{
	Properties
	{
		[HideInInspector] __dirty( "", Int ) = 1
		[HideInInspector]_SpecColor("SpecularColor",Color)=(1,1,1,1)
		_Smoothness("Smoothness", Range( 0 , 1)) = 1.54
		_Gloss("Gloss", Range( 0 , 1)) = 1.54
		_MainTex("MainTex", 2D) = "white" {}
		_NormalMap("NormalMap", 2D) = "bump" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityStandardUtils.cginc"
		#pragma target 2.0
		#pragma surface surf BlinnPhong keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _MainTex;
		uniform float4 _MainTex_ST;
		uniform float _Smoothness;
		uniform float _Gloss;

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			o.Normal = UnpackScaleNormal( tex2D( _NormalMap, uv_NormalMap ) ,0.0 );
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
7;29;1010;644;2162.635;743.6982;2.763519;True;False
Node;AmplifyShaderEditor.RangedFloatNode;7;-1299.81,460.799;Float;False;Constant;_NormalPower;Normal Power;1;0;0;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;5;-841.9216,231.2044;Float;False;Property;_Gloss;Gloss;1;0;1.54;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.RangedFloatNode;4;-835.5897,89.20972;Float;False;Property;_Smoothness;Smoothness;0;0;1.54;0;1;0;1;FLOAT
Node;AmplifyShaderEditor.SamplerNode;1;-821.0255,-127;Float;True;Property;_MainTex;MainTex;2;0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;COLOR;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SamplerNode;6;-860.237,386.9258;Float;True;Property;_NormalMap;NormalMap;3;0;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0.0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1.0;False;5;FLOAT3;FLOAT;FLOAT;FLOAT;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;2;-364,79;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-367.4418,180.6769;Float;False;2;2;0;FLOAT;0.0;False;1;FLOAT;0.0;False;1;FLOAT
Node;AmplifyShaderEditor.WireNode;9;-201.0271,325.5059;Float;False;1;0;FLOAT3;0.0;False;1;FLOAT3
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-32.02576,22.1717;Float;False;True;0;Float;ASEMaterialInspector;0;0;BlinnPhong;Mobile/ALIyerEdon/Realtime Specular + Normal Map ;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;Opaque;0.5;True;True;0;False;Opaque;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;14;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0.0;False;4;FLOAT;0.0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0.0;False;9;FLOAT;0.0;False;10;OBJECT;0.0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;5;7;0
WireConnection;2;0;1;4
WireConnection;2;1;4;0
WireConnection;8;0;1;4
WireConnection;8;1;5;0
WireConnection;9;0;6;0
WireConnection;0;0;1;0
WireConnection;0;1;9;0
WireConnection;0;3;2;0
WireConnection;0;4;8;0
ASEEND*/
//CHKSM=FB7200C2BC204709130ED258CD1BF9D4BFEFD3C7