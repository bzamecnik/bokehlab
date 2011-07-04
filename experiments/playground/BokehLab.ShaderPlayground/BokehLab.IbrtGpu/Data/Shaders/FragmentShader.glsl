#extension GL_EXT_gpu_shader4 : enable

// image layer - color
uniform sampler2D colorTexture;
// image layer - depth
uniform sampler2D depthTexture;

// precomputed pseudo-random senzor and lens sample positions
// - a 2D vector - lens position (x,y) in camera space
// - samples for one fragment stored in R dimension
// - samples for neighbor pixel tiled in S and T dimensions
// TODO: two 2D vectors could be packed into one 4D vector
uniform sampler3D lensSamples;
// number of samples - allowed values: [0; length(lensSamples)]
uniform int sampleCount;
uniform float sampleCountInv;

uniform float lensApertureRadius;
uniform float lensFocalLength;

// thin lens matrix for transforming from senzor space to object space
uniform mat4 thinLens;
// matrix for transforming from normalized senzor space to camera space
uniform mat3 senzorToCamera;
// matrix for transforming from camera space to normalized image layer space
uniform mat3 cameraToImage;

uniform float aspectRatio;
uniform float senzorDepth;

const float epsilon = 1e-6;

uniform float imageLayerDepth;
const float senzorWidth = 5.0;

//vec3 intersectPlane(vec3 rayOrigin, vec3 rayDir, vec3 planeOrigin, vec3 planeNormal)
//{
    //float t = dot((planeOrigin - rayOrigin), planeNormal) / dot(rayDir, planeNormal);
    //if (t < 0)
    //{
		//return null;
	//}    
    //vec3 intersectionPos = rayOrigin + t * rayDirection;
    //return intersectionPos;
//}

// TODO:
// - trace a single ray from the senzor through the thin lens to the layer image
//   - signature:
//     vec4 traceRay(vec2 senzorPos, vec2 lensPos);
//   - input:
//     - position on the senzor (two uniform numbers)
//       - computed from fragment position
//     - position on the lens (two position)
//       - pseudo-random
//     - constants
//       - senzor transform matrix (size, position, ...)
//       - image layer transform matrix (size, position, ...)
//       - thin lens matrix
//   - output:
//     - color of the scene sample
//   - algorithm:
//     - create a ray from senzor to lens
//       - get fragment coordinates - quad texture coords ([0;1]^2)
//       - transform them to 3D position in camera space
//         using senzor size, position [and rotation]
//         - this can be encoded in a single matrix
//     - transform the senzor position via thin lens matrix to its image
//     x- compute lens position from the parameters
//     - make a ray from lens to senzor position image
//     - intersect the ray with the image layer
//       - for a planar layer
//     - in case of intersection:
//       - transform the intersection position to image layer coordinates ([0;1]^2)
//       - obtain the color from the image texture and return it
//     - else return black color
//
// - make a Monte Carlo estimate of the incoming radiance going from the scene
//   through the lens to the current senzor pixel
//   - input:
//     - uniform:
//       - number of samples
//       - positions on the senzor and lens
//   - output:
//     - total color estimate for given fragment
//   - algorithm:
//     - for each sample in (1; sampleCount):
//       - get senzor and lens sample position from the texture
//       - trace ray made with the sampled positions
//       - accumulate the resulting color
//     - return the averaged accumulated color
//
// NOTE:
// Positions on the senzor and lens can be provided either as just uniform
// number and transformed in the shader or the transform can be done in the
// precomputation step. Two precomputed 2D vectors for each ray are enough.

vec4 traceRay(vec3 senzorPos, vec3 lensPos) {
	// transfer the ray through the lens
	//vec4 transformedPos = vec4(-senzorPos, 1);
	vec4 transformedPos = thinLens * vec4(senzorPos, 1.0);
	vec3 outputDir = transformedPos.xyz;
	if (abs(transformedPos.w) > epsilon)
    {
		outputDir /= transformedPos.w;
    }
    if (abs(transformedPos.w) > epsilon)
    {
        outputDir -= lensPos;
    }
    if (abs(senzorPos.z) <= lensFocalLength)
    {
        // the image of the incoming ray origin was in the same
        // half-plane as the origin itself
        outputDir = -outputDir;
    }
    
    // intersect planar image layer plane
    vec3 rayOrigin = lensPos;
    vec3 rayDir = outputDir - rayOrigin;
    //vec3 planeOrigin = vec3(0, 0, imageLayerDepth);
	//vec3 planeNormal = vec3(0, 0, 1);
    
    //float t = dot((planeOrigin - rayOrigin), planeNormal) / dot(rayDir, planeNormal);
    // when plane normal is (0,0,1) only z component is used:
    float t = (imageLayerDepth - rayOrigin.z) / rayDir.z;
    if (t < 0.0)
    {
		return vec4(1, 0, 1, 0); // no intersection
	}    
    vec3 intersectionPos = rayOrigin + t * rayDir;
    
    // TODO: transform to normalized image coordinates
    
    vec2 imagePos = intersectionPos.xy;
    //imagePos /= abs(imageLayerDepth);
    //imagePos = imagePos / vec2(1, aspectRatio) + vec2(0.5, 0.5);
    //imagePos += vec2(0.5, 0.5 * aspectRatio);
    imagePos /= abs(imageLayerDepth);
    imagePos += vec2(0.5, 0.5);
    
    //return vec4(imagePos, 0, 1);
    
    // retrieve color
    
    vec4 color = texture2D(colorTexture, imagePos);
    return color;
	
	//return vec4(0.5, 0, 0, 1);
	//return vec4(senzorPos.x, senzorPos.y, 0, 1);
	//return texture2D(colorTexture, senzorPos);
	//vec2 lens = texture1D(lensSamples, senzorPos.x).st;
	//vec2 lens = (lensPos / (lensApertureRadius * 2.0)) + 0.5;
	//return texture2D(colorTexture, senzorPos + 0.01 * lensPos);
	//return vec4(lens.s, lens.t, 0, 1);
}

vec3 getSenzorPos() {
	vec2 senzorPosNorm = gl_TexCoord[0].st;
	//ivec2 size = textureSize2D(colorTexture, 0);
	
	// shift half a pixel to pixel center
	//senzorPosNorm += 0.5 / vec2(size);
	// TODO: senzor position should probably be flipped as the image projected
	// onto it is flipped and we want to display it normally
	senzorPosNorm = vec2(1, 1) - senzorPosNorm;
	
	// transform senzor position from normalized senzor space to camera space
	//senzorPosNorm = (senzorPosNorm - vec2(0.5, 0.5)) * vec2(1, aspectRatio);
	//senzorPosNorm = senzorPosNorm - vec2(0.5, 0.5 * aspectRatio);
	senzorPosNorm -= vec2(0.5, 0.5);
	senzorPosNorm *= senzorDepth;
	//senzorPosNorm *= senzorWidth;
	vec3 senzorPos = vec3(senzorPosNorm, senzorDepth);
	return senzorPos;
}

vec4 estimateRadiance() {
	vec4 colorAccum = vec4(0, 0, 0, 0);
	// current pixel position on a senzor (at the pixel center)
	vec3 senzorPos = getSenzorPos();
	
	ivec3 lensJitterSize = textureSize3D(lensSamples, 0);
	ivec2 screenSize = textureSize2D(colorTexture, 0);
	vec2 jitterCoords = gl_FragCoord.st;
	jitterCoords.t = screenSize.y - jitterCoords.t;
	//vec3 samplesIndex = vec3(mod(gl_FragCoord.st / vec2(lensJitterSize.st), 1.0), 0.0);
	vec3 samplesIndex = vec3(jitterCoords / vec2(lensJitterSize.st), 0.0);
	vec3 samplesIndexStep = vec3(0, 0, 1.0 / (float(lensJitterSize.r) - 1.0));
	for (int i = 0; i < sampleCount; i += 1) {
		//vec4 params = texture1D(lensSamples, samplesIndex);
		//vec2 lensPos = params.xy;
		//vec2 senzorJitter = params.zw;
		//float samplesIndex = 0.0;
		vec2 lensPos = texture3D(lensSamples, samplesIndex).st;
		//vec2 lensPos = vec2(1, 0);
		colorAccum += traceRay(senzorPos, vec3(lensPos, 0));
		//return colorAccum;
		samplesIndex += samplesIndexStep;
	}
	return colorAccum * sampleCountInv;
	//return colorAccum;
}

//void blurGather() {
	//// integer texture size (width, height)
	//ivec2 size = textureSize2D(colorTexture, 0);
	//vec2 sizeInv = 1.0 / vec2(size);
	//vec2 nPos = gl_TexCoord[0].st;
	//vec2 iPos = gl_FragCoord.st;
	//
	//float radius = 10.0;
	////radius *= abs(texture2D(depthTexture, nPos).r - 0.5);
	//
	//vec2 iPosStart = iPos;
	//int sqrtCount = 4;
	//int count = 0;
	//float xyStep = radius / float(sqrtCount);
	//
	//vec3 color = vec3(0,0,0);
	//vec3 colorSum = vec3(0,0,0);
//
	//for (float y = -radius; y <= radius; y += xyStep) {
		//for (float x = -radius; x <= radius; x += xyStep) {
			//vec2 offset = vec2(x, y);
			//vec2 texpos = (iPos + offset) * sizeInv;
			//color = texture2D(colorTexture, texpos).rgb;
			//colorSum += color;
			//count++;
		//}
	//}
	//gl_FragColor.rgb = colorSum / float(count);
	//gl_FragColor.a = 1.0;
//}

void main() {
	gl_FragColor = estimateRadiance();
	
	//ivec3 lensJitterSize = textureSize3D(lensSamples, 0);
	////vec2 jitterCoords = gl_FragCoord.st / textureSize2D(colorTexture, 0);
	////jitterCoords.t = 1 - jitterCoords.t;
	//ivec2 screenSize = textureSize2D(colorTexture, 0);
	//vec2 jitterCoords = gl_FragCoord.st;
	//jitterCoords.t = screenSize.y - jitterCoords.t;
	//vec2 lensPos = texture3D(lensSamples, vec3(jitterCoords / vec2(lensJitterSize.st), 0.9)).st;
	////vec2 lensPos = texture3D(lensSamples, vec3(gl_TexCoord[0].st, 0.5)).st;
	//gl_FragColor = vec4((lensPos + 1) * 0.5, 0, 1);
	////gl_FragColor = vec4(jitterCoords / vec2(lensJitterSize.st), 0, 1);
	////gl_FragColor = vec4(jitterCoords, 0, 1);
}
