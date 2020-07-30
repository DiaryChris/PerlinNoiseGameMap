Shader "Custom/NewTextureBlending" {
	Properties {
		_MainTint("Diffuse Tint", Color) = (1,1,1,1)

		//Add the properties below so we can input all of our textures
		_ColorA("Terrain Color A", Color) = (1,1,1,1)
		_ColorB("Terrain Color B", Color) = (1,1,1,1)
		_RTexture("Red Channel Texture", 2D) = ""{}
		_GTexture("Green Channel Texture", 2D) = ""{}
		_BTexture("Blue Channel Texture", 2D) = ""{}
		_ATexture("Alpha Channel Texture", 2D) = ""{}
		_BlendTex("Blend Texture", 2D) = ""{}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma surface surf Lambert

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 4.0

		float4 _MainTint;
		float4 _ColorA;
		float4 _ColorB;
		sampler2D _RTexture;
		sampler2D _GTexture;
		sampler2D _BTexture;
		sampler2D _ATexture;
		sampler2D _BlendTex;

		struct Input {
			float2 uv_RTexture;
			float2 uv_GTexture;
			float2 uv_BTexture;
			float2 uv_ATexture;
			float2 uv_BlendTex;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			

			float4 blendData = tex2D(_BlendTex, IN.uv_BlendTex);

			//Get the data from the textures we want to blend
			float4 rTexData = tex2D(_RTexture, IN.uv_RTexture);
			float4 gTexData = tex2D(_GTexture, IN.uv_GTexture);
			float4 bTexData = tex2D(_BTexture, IN.uv_BTexture);
			float4 aTexData = tex2D(_ATexture, IN.uv_ATexture);

			float4 c;
			c = lerp(rTexData, gTexData, blendData.g);
			c = lerp(c, bTexData, blendData.b);
			c = lerp(c, aTexData, blendData.a);

			//Add on our terrain tinting colors
			float4 terrainLayers = lerp(_ColorA, _ColorB, blendData.r);
			c *= terrainLayers;
			c = saturate(c);

			o.Albedo = c.rgb * _MainTint.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
	FallBack "Diffuse"
}
