// NOTE: quite a bad random number generator
// - it has visible periodic artifacts
// - the question is if it is bad for lens sampling

// http://www.geeks3d.com/20100831/shader-library-noise-and-pseudo-random-number-generator-in-glsl/
float rnd(vec2 x)
{
    //int n = int(x.x * 40.0 + x.y * 6400.0);
    //n = (n << 13) ^ n;
    //return 1.0 - float( (n * (n * n * 15731 + 789221) + 1376312589) & 0x7fffffff) / 1073741824.0;
}

void main() {
	gl_FragColor.rgb = rnd(gl_FragCoord);
	gl_FragColor.a = 1;
}
