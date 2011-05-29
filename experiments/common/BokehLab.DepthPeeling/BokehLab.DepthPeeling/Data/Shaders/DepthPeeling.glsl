//uniform sampler2DRectShadow depthTexture;
uniform sampler2D depthTexture;
//uniform sampler2D colorTexture;

void main() {
	// fetch depth of fragment from previous layer at the same position
	//int depth = shadow2DRect(depthTexture, gl_FragCoord.xyz).x;
	//int depth = texture2D(depthTexture, gl_FragCoord.xy).x;
	//if (gl_FragCoord.z <= depth) {
	
	float depth = texture2D(depthTexture, gl_FragCoord.xy/(float)512);
	if (depth >= gl_FragCoord.z) {
		discard;
	}
	
	gl_FragColor = gl_Color;
	
	//gl_FragColor = texture2D(depthTexture, gl_FragCoord.xy/(float)512);
	
	//gl_FragColor = gl_FragCoord.x/(float)512;
}