// Crest Ocean System

// Copyright 2020 Wave Harmonic Ltd

#include "OceanGraphConstants.hlsl"
#include "../OceanGlobals.hlsl"

void CrestNodeOceanGlobals_float
(
	out float o_crestTime,
	out float o_texelsPerWave,
	out float3 o_oceanCenterPosWorld,
	out float o_sliceCount,
	out float o_meshScaleLerp
)
{
	o_crestTime = _CrestTime;
	// TODO: Look at removing as I do not think it is used.
	o_texelsPerWave = 2.0;
	o_oceanCenterPosWorld = _OceanCenterPosWorld;
	o_sliceCount = _SliceCount;
	o_meshScaleLerp = _MeshScaleLerp;
}
