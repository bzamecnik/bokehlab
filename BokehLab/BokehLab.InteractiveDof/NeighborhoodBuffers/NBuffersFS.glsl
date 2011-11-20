#version 120
#extension GL_EXT_texture_array : enable
//#extension GL_EXT_gpu_shader4 : enable

// produces the next min/max N-buffer level either from the previous N-buffer level

uniform sampler2D prevLevelMinTexture; // outside clamped to 1
uniform sampler2DArray prevLevelMaxTexture; // outside clamped to 0
// offset for the current level in the texture coordinates (x, y, 0)
uniform vec3 offset;
uniform int prevLevel;

void main() {
	vec3 coords = vec3(gl_TexCoord[0].st, prevLevel);
	// single texture2D -> 2D coords
	float depthAmin = texture2D(prevLevelMinTexture, coords.xy).r;
	float depthBmin = texture2D(prevLevelMinTexture, coords.xy + offset.xz).r;
	float depthCmin = texture2D(prevLevelMinTexture, coords.xy + offset.zy).r;
	float depthDmin = texture2D(prevLevelMinTexture, coords.xy + offset.xy).r;
	
	float minValue = min(min(depthAmin, depthBmin), min(depthCmin, depthDmin));
	
	// texture2D array -> 3D coords (z is from [0; layerCount-1], not [0.0; 1.0]
	float depthAmax = texture2DArray(prevLevelMaxTexture, coords).g;
	float depthBmax = texture2DArray(prevLevelMaxTexture, coords + offset.xzz).g;
	float depthCmax = texture2DArray(prevLevelMaxTexture, coords + offset.zyz).g;
	float depthDmax = texture2DArray(prevLevelMaxTexture, coords + offset.xyz).g;
	
	float maxValue = max(max(depthAmax, depthBmax), max(depthCmax, depthDmax));
	
	gl_FragColor = vec4(minValue, maxValue, 0, 0);
	//gl_FragColor = vec4(minValue, 0, 0, 0);
	//gl_FragColor = vec4(0, maxValue, 0, 0);
	//gl_FragColor = vec4(depthDmin, depthDmax, 0, 0);
}