{
	$GooObjectCommon(obj=clip)$
	
	"binaryRef": "$clip.Binary.BinaryId$",
	
	"channels": { $clip.Channels:{
		"$it.Id$": {
			"blendType": "Linear",
			"jointIndex": $it.Bone.Index$,
			"name": "$it.Bone.Name$",
			"type": "Joint",
			
			"times": $it.TimesRef$,
			"translationSamples": $it.TranslationSamplesRef$,
			"rotationSamples": $it.RotationSamplesRef$,
			"scaleSamples": $it.ScaleSamplesRef$,
		},}$
	}
}