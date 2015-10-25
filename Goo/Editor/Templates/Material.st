{
	$GooObjectCommon(obj=material)$
		
	"wireframe": $material.IsWireframe$,
	"flat": $material.IsFlat$,
	"blendState": {
		"blending": "$material.Blending$",
		"blendEquation": "$material.BlendEquation$",
		"blendSrc": "$material.BlendSource$",
		"blendDst": "$material.BlendDestination$"
	},
	"cullState": {
		"enabled": $material.Culling$,
		"cullFace": "$material.CullFace$",
		"frontFace": "$material.FrontFace$"
	},
	"depthState": {
		"enabled": $material.DepthTest$,
		"write": $material.DepthWrite$,
	},
	"dualTransparency": false,
	"renderQueue": -1,
	
	"shaderRef": "GOO_ENGINE_SHADERS/uber",
	
	"texturesMapping": { $material.BoundTextures: {slot|
	
		"$slot.Key$": {
			"textureRef": "$slot.Value$",
			"enabled": true
		},}$
	},
	
	"uniforms": { $material.Uniforms: {uniform|
		
		"$uniform.Name$": {
			"enabled": true,
			"value": $uniform.Value$
		},}$
	}
}

