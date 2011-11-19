uniform sampler2D nBufferLayerTexture;
uniform vec2 colorMask;

void main() {
	vec2 coords = gl_TexCoord[0].st;
	
	vec2 value = texture2D(nBufferLayerTexture, coords).rg;
	
	gl_FragColor = vec4(value * colorMask, 0, 1);
	//gl_FragColor = vec4(1, 0, 1, 1);
}
