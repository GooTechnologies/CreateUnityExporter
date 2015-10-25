{
	$GooObjectCommon(obj=mesh)$
	
	$if(mesh.IsBinary)$	"binaryRef":"$mesh.BinaryId$",$endif$
	"type": "$if(mesh.IsSkinned)$SkinnedMesh$else$Mesh$endif$",

	"indexModes": ["Triangles"],
	"indexLengths": [$mesh.IndexCount$],
	"indices": $mesh.IndicesPtr$,
	
	"vertexCount": $mesh.VertexCount$,
	"attributes": {
		"POSITION": { "value": $mesh.PositionsPtr$, "dimensions": 3 },
		"NORMAL": { "value": $mesh.NormalsPtr$, "dimensions": 3 },
		"TEXCOORD0": { "value": $mesh.TexCoords0Ptr$, "dimensions": 2 },
		"TEXCOORD1": { "value": $mesh.TexCoords1Ptr$, "dimensions": 2 },
		$if(mesh.HasVertexColors)$
		"COLOR": { "value": $mesh.ColorsPtr$, "dimensions": 4 },
		$endif$
		$if(mesh.BoneIndicesPtr)$
		"JOINTIDS": { "value": $mesh.BoneIndicesPtr$, "dimensions": $mesh.SkinningComponentCount$ },
		$endif$
		$if(mesh.BoneWeightsPtr)$
		"WEIGHTS": { "value": $mesh.BoneWeightsPtr$, "dimensions": $mesh.SkinningComponentCount$ },
		$endif$
	},
	
	"boundingVolume": {
		"type": "BoundingBox",
		"center": [$mesh.BoundingBox.center.x$, $mesh.BoundingBox.center.y$, $mesh.BoundingBox.center.z$],
		"min": [$mesh.BoundingBox.min.x$, $mesh.BoundingBox.min.y$, $mesh.BoundingBox.min.z$],
		"max": [$mesh.BoundingBox.max.x$, $mesh.BoundingBox.max.y$, $mesh.BoundingBox.max.z$]
	}
}