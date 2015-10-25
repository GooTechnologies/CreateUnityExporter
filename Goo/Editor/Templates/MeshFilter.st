"meshData": {
	"meshRef": "$it.MeshId$",
	$if(it.Skeleton)$
	"poseRef": "$it.Skeleton.Id$",
	$endif$
},