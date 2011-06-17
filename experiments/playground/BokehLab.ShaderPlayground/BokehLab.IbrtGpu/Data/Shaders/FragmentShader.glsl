uniform sampler2D colorTexture;
uniform sampler2D depthTexture;

void main() {
	vec3 color = texture2D(colorTexture, gl_TexCoord[0].st);
	vec3 depth = texture2D(depthTexture, gl_TexCoord[0].st);
	gl_FragColor = vec4(1 - depth.r, color.g, color.b, 1);
}