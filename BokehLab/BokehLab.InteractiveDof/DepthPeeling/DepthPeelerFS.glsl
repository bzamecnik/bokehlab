#version 120
#extension GL_EXT_texture_array : enable

uniform sampler2DArray depthTexture;
uniform int prevLayer;
uniform vec2 depthTextureSizeInv;

uniform sampler2D texture0;
//varying vec3 normal;
//varying vec3 view;

// NOTE: We merged the actual scene shader here. Better the Scene itself
// should be responsible for the shading. We might want multiple shaders
// for different effects. They have to be combined with depth peeling.
vec4 shadeFragment() {
	//return gl_Color;
	vec3 texColor = texture2D(texture0, gl_TexCoord[0].st).rgb;
	return vec4(texColor, 1);
	
	////vec3 light = normalize(vec3(1,1,1) - view);
	//vec3 light = normalize(vec3(1,1,1));
	////vec4 diffuse = clamp(texColor * max(dot(normal, light), 0.0) 0.0, 1.0);
	//float diffuse = max(dot(normal, light), 0.0);
	////return vec4(diffuse, diffuse, diffuse, 1);
	//float ambient = 0.2;
	//vec3 color = (ambient + diffuse) * texColor;
	//return vec4(color, 1);
}

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