"camera": {
	"near": $it.Near$,
	"far": $it.Far$,
	"aspect": $it.AspectRatio$,
	"lockedRatio": false,
	"projectionMode": "$it.ProjectionMode$",
	$if(it.IsPerspective)$
	"fov": $it.FOV$,
	$else$
	"size": $it.OrthographicSize$,
	$endif$
},