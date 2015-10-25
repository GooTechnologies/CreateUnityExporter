{
	$GooObjectCommon(obj=animation)$
	
	"layers": {
		"$animation.LayerId$": {
			"sortValue": 0,
			"id": "$animation.LayerId$",
			"name": "Default",
			"blendWeight": 1,
			
			$if(animation.DefaultState)$
			"initialStateRef": "$animation.DefaultState.Id$",
			$endif$
			
			"states": { $animation.NumberedStates: {kvp|
			
				"$kvp.Value.Id$": {
					"sortValue": $kvp.Key$,
					"stateRef": "$kvp.Value.Id$",
				},
		  	}$},
		  	"transitions": {
		  		"*":{
		  			"type": "Fade",
		  			"fadeTime": 0
		  		}
		  	}
		}
	}
}

