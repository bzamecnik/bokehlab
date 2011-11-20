// queries the min/max N-buffer

uniform sampler2DArray nBuffersTexture;
// offset for the current level in the texture coordinates (x, y, 0)
uniform vec3 offset;
// N-buffers size in pixels (width, height)
uniform vec2 nBuffersSize;

// get min/max values in a rectangular window (pos, pos + size) in [0.0; 1.0]^2
vec2 queryMinMaxNBuffers(vec2 position, vec2 size)
	vec2 coords = gl_TexCoord[0].st;
	vec2 rectSizeInPixels = size * nBuffersSize;
	int level = int(ceil(log(max(rectSizeInPixels.x, rectSizeInPixels.y), 2)));
	vec2 minmax = texture2DArray(nBuffersTexture, vec3(coords, level)).rg;		
	return minmax;
}
