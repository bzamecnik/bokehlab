void main() {
	gl_FragColor.rg = 0.5 * (sin(2 * 3.1415926535 * ( 0.01 * width) * gl_TexCoord[0].st) + 1);
	gl_FragColor.ba = vec2(0, 1);
}