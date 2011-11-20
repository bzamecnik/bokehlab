#version 120
#extension GL_EXT_texture_array : enable

// produces the next min/max N-buffer level either from the original packed
// depth texture taking the extreme value from each vector component

uniform sampler2DArray packedDepthTexture;
uniform int layer;

float vecMin(vec4 vector) {
	return min(min(vector.x, vector.y), min(vector.z, vector.w));
}

float vecMax(vec4 vector) {
	return max(max(vector.x, vector.y), max(vector.z, vector.w));
}

void main() {
	vec2 coords = gl_TexCoord[0].st;

	vec4 depth = texture2DArray(packedDepthTexture, vec3(coords, layer));
	float minValue = vecMin(depth);
	float maxValue = vecMax(depth);
	//float minValue = depth.x; // visualize just the first layers
	//float maxValue = depth.x;
	
	gl_FragColor = vec4(minValue, maxValue, 0, 0);
	
	//gl_FragColor = vec4(depth.rgb, 1);
	//gl_FragColor = vec4(1, 1, 0, 1);
}
