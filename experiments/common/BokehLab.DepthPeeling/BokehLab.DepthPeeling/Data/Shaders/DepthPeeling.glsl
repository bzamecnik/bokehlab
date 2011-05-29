//uniform sampler2DRectShadow depthTexture;
//uniform sampler2DShadow depthTexture;
//uniform sampler2DRect depthTexture;
uniform sampler2D depthTexture;
//uniform sampler2D colorTexture;

void main() {
	// fetch depth of fragment from previous layer at the same position
	//float depth = shadow2DRect(depthTexture, gl_FragCoord.xyz).x;
	//float depth = texture2D(depthTexture, gl_FragCoord.xy).x;
	//if (gl_FragCoord.z <= depth) {
	
	//float depth = texture2DRect(depthTexture, gl_FragCoord.xy);
	//if (depth >= gl_FragCoord.z) {
		//discard;
	//}
	
	//vec4 depth = shadow2D(depthTexture, gl_FragCoord.xyz/(float)512);
	//gl_FragColor = depth;
	//if (depth > 0.5) {
		//discard;
	//}
	
	//// code for sampler2D
	float depth = texture2D(depthTexture, gl_FragCoord.xy/(float)512);
	if (depth >= gl_FragCoord.z) {
		discard;
	}
	
	gl_FragColor = gl_Color;
	
	//gl_FragColor = texture2D(depthTexture, gl_FragCoord.xy/(float)512);
	
	//gl_FragColor = gl_FragCoord.x/(float)512;
}