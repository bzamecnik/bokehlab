void main() {
	// using fragment coordinates to get top-bottm, left-right directions
	vec2 texCoord = gl_FragCoord.st / textureSize2D(colorTexture, 0);
	gl_FragColor = vec4(0, texCoord.s, 1 - texCoord.t, 0);
}

void main() {
	// using texture coordinates to get top-bottm, left-right directions
	gl_FragColor.gb = gl_TexCoord[0].st;
	gl_FragColor.ra = vec2(0, 1);
}
