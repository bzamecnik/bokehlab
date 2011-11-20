//varying vec3 normal;
//varying vec3 view;

void main() {
	gl_Position = ftransform();
	gl_TexCoord[0] = gl_MultiTexCoord0;
	gl_FrontColor = gl_Color;
	//view = vec3(gl_ModelViewMatrix * gl_Vertex);       
    //normal = normalize(gl_NormalMatrix * gl_Normal);
}