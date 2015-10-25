"light": {
	"type": "$it.TypeOfLight$",
	"color": [ $it.Color; separator=", "$ ],
	"intensity": $it.Intensity$,
	"specularIntensity": $it.SpecularIntensity$,
	"range": $it.Range$,
	
	$if(it.IsSpotLight)$
	"angle": $it.SpotAngle$,
	"exponent": $it.SpotAngle$,
	"direction": [ $it.Direction; separator=", "$ ],
	$endif$
	
	$if(it.IsDirectionalLight)$
	"direction": [ $it.Direction; separator=", "$ ],
	$endif$
	
	"shadowCaster": $if(it.IsCastingShadows)$true$else$false$endif$,
	
	$if(it.IsCastingShadows)$
	"shadowSettings": {
		"darkness":$it.Darkness$,
		"far":$it.Far$,
		"near":$it.Near$,
		$if(it.IsDirectionalLight)$
		"size":$it.Size$,
		$endif$
		"resolution":[ $it.ShadowResolution; separator=", "$ ],
		"shadowType": "$it.ShadowType$"
	},
	$endif$
	
	$if(it.LightCookieEnabled)$
	"lightCookie": {
		"enabled":true,
		"textureRef":"$it.LightCookieTextureRef$"
	},$endif$
},