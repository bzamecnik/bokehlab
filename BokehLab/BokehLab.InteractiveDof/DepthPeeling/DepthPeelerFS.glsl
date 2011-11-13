uniform sampler2D depthTexture;
uniform vec2 depthTextureSizeInv;

void main() {
	float depth = texture2D(depthTexture, gl_FragCoord.xy * depthTextureSizeInv);
	//if ((depth == 1.0) || (gl_FragCoord.z <= depth)) {
	if (gl_FragCoord.z <= depth) {
		discard;
	}
	
	gl_FragColor = gl_Color;
	// use 1.0 for "undefined" value and move fragments from the far plane at 1.0 to 1.0 - epsilon
	//gl_FragDepth = min(gl_FragCoord.z, 0.999);
}