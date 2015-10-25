{
	$GooObjectCommon(obj=skeleton)$
	
	"joints": { $skeleton.Bones:{bone|
	
		"$bone.Id$": {
			"index": $bone.Index$,
			"parentIndex": $if(bone.IsRootBone)$-32768$else$$bone.ParentIndex$$endif$,
			"name": "$bone.Name$",
			"inverseBindPose": $bone.InverseBindPoseMatrix$,
		},
	}$}
}