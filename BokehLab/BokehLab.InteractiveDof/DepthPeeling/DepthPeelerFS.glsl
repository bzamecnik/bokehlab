uniform sampler2D depthTexture;
uniform vec2 depthTextureSizeInv;

uniform sampler2D texture0;

// NOTE: We merged the actual scene shader here. Better the Scene itself
// should be responsible for the shading. We might want multiple shaders
// for different effects. They have to be combined with depth peeling.
vec4 shadeFragment() {
	//return gl_Color;
	return texture2D(texture0, gl_TexCoord[0].st);
}

void main() {
	float depth = texture2D(depthTexture, gl_FragCoord.xy * depthTextureSizeInv);
	//if ((depth == 1.0) || (gl_FragCoord.z <= depth)) {
	if (gl_FragCoord.z <= depth) {
		discard;
	}
	
	gl_FragColor = shadeFragment();
	// use 1.0 for "undefined" value and move fragments from the far plane at 1.0 to 1.0 - epsilon
	//gl_FragDepth = min(gl_FragCoord.z, 0.999);
}