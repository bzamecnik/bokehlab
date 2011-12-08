#version 120
#extension GL_EXT_texture_array : enable

uniform sampler2DArray depthTexture;
uniform int prevLayer;
uniform vec2 depthTextureSizeInv;

vec4 shadeFragment();

void main() {
	float depth = texture2DArray(depthTexture, vec3(gl_FragCoord.xy * depthTextureSizeInv, prevLayer)).r;
	//if ((depth == 1.0) || (gl_FragCoord.z <= depth)) {
	if (gl_FragCoord.z <= depth) {
		discard;
	}
	
	gl_FragColor = shadeFragment();
	// use 1.0 for "undefined" value and move fragments from the far plane at 1.0 to 1.0 - epsilon
	//gl_FragDepth = min(gl_FragCoord.z, 0.999);
}