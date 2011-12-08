#version 120
#extension GL_EXT_texture_array : enable

uniform sampler2DArray depthTexture;

void main() {
	vec2 coords = gl_TexCoord[0].st;
	// offset of the first layer (in case there are more 4-batches than one)
	int firstLayerOffset = 0;
	//float undefinedValue = 0.0;
	//vec4 packedDepth = vec4(undefinedValue);
	vec4 packedDepth = vec4(
		texture2DArray(depthTexture, vec3(coords, firstLayerOffset)).r,
		texture2DArray(depthTexture, vec3(coords, firstLayerOffset + 1)).r,
		texture2DArray(depthTexture, vec3(coords, firstLayerOffset + 2)).r,
		texture2DArray(depthTexture, vec3(coords, firstLayerOffset + 2)).r
	);
	gl_FragColor = packedDepth;
	
	// visualization of the first three layers
	//gl_FragColor = vec4(1.0 - packedDepth.rgb, 1.0);
}