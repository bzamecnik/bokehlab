#version 120
#extension GL_EXT_texture_array : enable
//#extension GL_EXT_gpu_shader4 : enable

// produces the next min/max N-buffer level either from the previous N-buffer level

uniform sampler2DArray prevLevelTexture; // outside clamped to (1, 0) for min/max
// offset for the current level in the texture coordinates (x, y, 0)
uniform vec3 offset;
uniform int prevLevel;

void main() {
	vec3 coords = vec3(gl_TexCoord[0].st, prevLevel);

	// texture2D array -> 3D coords (z is from [0; layerCount-1], not [0.0; 1.0]
	vec2 depthA = texture2DArray(prevLevelTexture, coords).rg;
	vec2 depthB = texture2DArray(prevLevelTexture, coords + offset.xzz).rg;
	vec2 depthC = texture2DArray(prevLevelTexture, coords + offset.zyz).rg;
	vec2 depthD = texture2DArray(prevLevelTexture, coords + offset.xyz).rg;
	
	float minValue = min(min(depthA.r, depthB.r), min(depthC.r, depthD.r));
	float maxValue = max(max(depthA.g, depthB.g), max(depthC.g, depthD.g));
	
	gl_FragColor = vec4(minValue, maxValue, 0, 0);
	//gl_FragColor = vec4(minValue, 0, 0, 0);
	//gl_FragColor = vec4(0, maxValue, 0, 0);
	//gl_FragColor = vec4(depthDmin, depthDmax, 0, 0);
}