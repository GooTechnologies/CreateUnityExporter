"sound":{
	"volume":$it.Volume$,

	$if(it.HasAssociatedClip)$
	"sounds": {
		"$it.SoundRef$": {
		"soundRef":"$it.SoundRef$",
		"sortValue":0
		},
	},
	$endif$
},