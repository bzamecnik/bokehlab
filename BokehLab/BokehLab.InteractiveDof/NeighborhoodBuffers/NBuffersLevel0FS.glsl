#version 120
#extension GL_EXT_texture_array : enable

// produces the next min/max N-buffer level either from the original packed
// depth texture taking the extreme value from each vector component

uniform sampler2DArray packedDepthTexture;
uniform int layer;

void main() {
	vec2 coords = gl_TexCoord[0].st;

	vec4 depth = texture2DArray(packedDepthTexture, vec3(coords, layer));
	//float minValue = vecMin(depth);
	//float maxValue = vecMax(depth);
	float minValue = min(min(depth.x, depth.y), min(depth.z, depth.w));
	float maxValue = max(max(depth.x, depth.y), max(depth.z, depth.w));
	//float minValue = depth.x; // visualize just the first layers
	//float maxValue = depth.x;
	
	gl_FragColor = vec4(minValue, maxValue, 0, 0);
	
	//gl_FragColor = vec4(depth.rgb, 1);
	//gl_FragColor = vec4(1, 1, 0, 1);
}
