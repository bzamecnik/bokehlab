uniform sampler2D depthTexture;
uniform vec2 depthTextureSizeInv;

void main() {
	float depth = texture2D(depthTexture, gl_FragCoord.xy * depthTextureSizeInv);
	//
	//float depth = texture2D(depthTexture, gl_TexCoord[0].st);
	//
	//if ((depth == 1) || (gl_FragCoord.z <= depth)) {
	if (gl_FragCoord.z <= depth) {
		discard;
	}
	
	gl_FragColor = gl_Color;
}