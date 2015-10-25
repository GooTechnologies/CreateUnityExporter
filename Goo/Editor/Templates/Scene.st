{
	$GooObjectCommon(obj=scene)$

  	"entities": {$entities:{ent|
  	
  		"$ent.Value.Id$": {
  			"entityRef": "$ent.Value.Id$",
  			"sortValue": $ent.Key$,
  		},}$
  	},
  	
  	"initialCameraRef": "$scene.DefaultCamera.GlexGameObject.Id$",
  	$if(scene.Environment)$
  	"environmentRef": "$scene.Environment.Id$",
  	$endif$
}

