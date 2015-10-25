"animation": {
	"layersRef": "$it.Id$",
	$if(it.Skeleton)$
	"poseRef": "$it.Skeleton.Id$",
	$endif$
},