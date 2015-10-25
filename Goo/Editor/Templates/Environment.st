{
	$GooObjectCommon(obj=env)$
	
	"backgroundColor": $env.BackgroundColor$,
	"globalAmbient": $env.AmbientColor$,

	$if(env.Skybox)$
	"skyboxRef": "$env.Skybox.Id$",
	$endif$

	"fog": {
		"enabled": $env.IsFogEnabled$,
		"color": $env.FogColor$,
		"near": $env.FogNear$,
		"far": $env.FogFar$
	}
}
