uniform sampler2D texture0;

vec4 shadeFragment() {
	vec3 texColor = texture2D(texture0, gl_TexCoord[0].st).rgb;
	return vec4(texColor, 1);
}
