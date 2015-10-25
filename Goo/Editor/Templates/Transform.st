"transform": {
	"translation": $it.Translation$,
	"rotation": $it.RotationAsEuler$,
	"scale": $it.Scale$,
	$if(it.Entity.HasChildren)$
	"children": { $it.Entity.NumberedChildren:{kvp|
		
		"$kvp.Value.Id$": {
			"entityRef": "$kvp.Value.Id$",
			"sortValue": $kvp.Key$
		},}$
	},
	$endif$
},