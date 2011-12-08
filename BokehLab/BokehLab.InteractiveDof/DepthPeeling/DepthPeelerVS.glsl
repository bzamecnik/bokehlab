varying vec3 normal;
varying vec3 vertex; // vertex position in eye space

void main() {
	gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_FrontColor = gl_Color;
	vertex = (gl_ModelViewMatrix * gl_Vertex).xyz;
    normal = normalize(gl_NormalMatrix * gl_Normal);
}