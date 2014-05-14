// Unlit shader. Simplest possible textured shader.
// - SUPPORTS lightmap
// - no lighting
// - no per-material color

Shader "Mobile/LNUnlitLightmap" {
Properties {
//	_Color ("Main Color", Color) = (1.0, 1.0, 1.0, 1.0) // lunafish
	_MainTex ("Base (RGB)", 2D) = "white" {}
	_LightMap ("Lightmap (RGB)", 2D) = "lightmap" { LightmapMode }
}

SubShader {
	Tags { "RenderType"="Opaque" }
	LOD 100

	// lunafish
	Pass {
		Tags { "LightMode" = "Vertex" }
		
		Lighting Off
		
		BindChannels {
			Bind "Vertex", vertex
			Bind "texcoord1", texcoord0 // lightmap uses 2nd uv
			Bind "texcoord", texcoord1 // main uses 1st uv
		}		
		
//		SetTexture [_LightMap] {
//			constantColor [_Color]
//			combine texture * constant
//		}
		
		// combine lightmap
		SetTexture [_LightMap] { combine texture }

		// make result
		SetTexture [_MainTex] {
			combine texture * previous, texture * primary
		}
	}
}


}



