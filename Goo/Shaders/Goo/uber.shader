Shader "Goo/uber" {
	Properties {
		diffuseMap ("Diffuse Map", 2D) = "white" {}
		normalMap ("Normal Map", 2D) = "white" {}
		specularMap ("Specular Map", 2D) = "white" {}
		emissiveMap ("Emissive Map", 2D) = "white" {}
		AOMap ("AO Map", 2D) = "white" {}
		lightMap ("Light Map", 2D) = "white" {}

		materialAmbient ("Ambient Color", Color) = (0.5, 0.5, 0.5, 1.0)
		materialDiffuse ("Diffuse Color", Color) = (1.0, 1.0, 1.0, 1.0)
		materialSpecular ("Specular Color", Color) = (0.8, 0.8, 0.8, 1.0)
		materialEmissive ("Emissive Color", Color) = (0.0, 0.0, 0.0, 1.0)
		materialSpecularPower ("Specular Power", Float) = 30
		opacity ("Opacity", Float) = 1.0
	}
	SubShader {
		Tags { "Queue"="Geometry" }
		LOD 200
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D diffuseMap;

		struct Input {
			float2 uvdiffuseMap;
		};

		void surf (Input IN, inout SurfaceOutput o) {
			half4 c = tex2D (diffuseMap, IN.uvdiffuseMap);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}


