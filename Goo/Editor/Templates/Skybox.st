{
	$GooObjectCommon(obj=skybox)$
	
	"box": {
		"enabled": true,
		$skybox.top:{tex| "topRef": "$tex.Id$",}$
		$skybox.bottom:{tex| "bottomRef": "$tex.Id$",}$
		$skybox.left:{tex| "leftRef": "$tex.Id$",}$
		$skybox.right:{tex| "rightRef": "$tex.Id$",}$
		$skybox.front:{tex| "frontRef": "$tex.Id$",}$
		$skybox.back:{tex| "backRef": "$tex.Id$",}$
	}
}
