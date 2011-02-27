from spherical import SphericalCoords
from frame import *
from visual import *

def visualizeSphereParametrization():
    sphere(radius=1, opacity=0.25, material=materials.show_mat_pos)
    thetaSampleCount = 20
    phiSampleCount = 40
    thetaStep = 1.0 / float(thetaSampleCount)
    phiStep = 1.0 / float(phiSampleCount)
    for i in range(0, thetaSampleCount + 1):
        for j in range(0, phiSampleCount + 1):
            spherical = SphericalCoords(
                theta = i * thetaStep * math.pi,
                phi = j * phiStep * 2 * math.pi)
            color = visual.color.hsv_to_rgb((j * phiStep, i * thetaStep, 1.0))
            points(pos=[spherical.toCartesian()], color=color)
    
drawWorldCoordinates()
visualizeSphereParametrization()
