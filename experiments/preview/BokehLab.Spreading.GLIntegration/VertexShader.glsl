//#version 140
 //
//uniform Transformation {
    //mat4 projection_matrix;
    //mat4 modelview_matrix;
//};
 //
//in vec3 vertex;
//in vec4 gl_Color;
 //
//void main() {
    //gl_Position = projection_matrix * modelview_matrix * vec4(vertex, 1.0);
    //gl_FrontColor = gl_Color;
//}

void main() {
	gl_Position = ftransform();
	gl_FrontColor = gl_Color;
}
