varying vec3 normal;
varying vec3 vertex;

uniform float diffuseCoeff;
uniform float ambient;
uniform vec3 baseColor;

vec4 shadeFragment() {
	vec3 lightDir = normalize(vec3(1, 1, 1) - vertex);
	float diffuse = clamp(diffuseCoeff * max(dot(normal, lightDir), 0.0), 0.0, 1.0);
	vec3 color = (ambient + diffuse) * baseColor;
	return vec4(color, 1);
}
