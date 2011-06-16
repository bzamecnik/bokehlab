//uniform sampler2D foo2dTexture;
uniform sampler3D foo3dTexture;
uniform float zCoord;
uniform vec4 valueMask;

void main() {
	//gl_FragColor = texture2D(foo2dTexture, gl_TexCoord[0].st);
	
	float s = gl_TexCoord[0].s;
	float t = gl_TexCoord[0].t;
	//vec4 lrtfValue = texture3D(foo3dTexture, vec3(zCoord, t, s));
	vec4 lrtfValue = texture3D(foo3dTexture, vec3(s, t, zCoord));
	float intensity = dot(lrtfValue, valueMask);
	gl_FragColor.rgb = intensity;
	gl_FragColor.a = 1;
}