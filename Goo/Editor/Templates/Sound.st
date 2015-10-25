{
	$GooObjectCommon(obj=sound)$

	"loop":$if(sound.IsLooping)$true$else$false$endif$,
	"volume":$sound.Volume$,
	"duration":$sound.Duration$,
	"audioRefs": {
		"$sound.Format$" : "$sound.BinaryId$"
	}
}
