import math
from math import sin, cos, acos, atan2

class SphericalCoords(object):
    def __init__(self, theta=0.0, phi=0.0):
        self.theta = theta
        self.phi = phi

    def fromCartesian(self, cartesian, normalized=True):
        x = cartesian[0]
        y = cartesian[1]
        z = cartesian[2]
        rInv = normalized and 1.0 or 1.0 / norm((x, y, z))
        theta = acos(z * rInv)
        phi = atan2(y, x)
        return SphericalCoords(theta, phi)

    def toCartesian(self):
        sinTheta = sin(self.theta)
        x = sinTheta * cos(self.phi)
        y = sinTheta * sin(self.phi)
        z = cos(self.theta)
        return (x, y, z)

    def reflect(self):
        return SphericalCoords(-self.theta, -self.phi)
