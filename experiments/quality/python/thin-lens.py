import visual
import visual.controls
import math
import transform
import frame
import spherical

class ThinLens(object):
    def __init__(self, focalDistance, apertureRadius):
        self.focalDistance = focalDistance
        self.apertureRadius = apertureRadius
        self.transformMatrix = self.createTransformMatrix(self.focalDistance)

    def createTransformMatrix(self, focalDistance):
        f = focalDistance
        return visual.array([
            [f, 0, 0, 0],
            [0, f, 0, 0],
            [0, 0, f, 0],
            [0, 0, 1, f]
        ])

    def transform(self, vector):
        return transform.transform(self.transformMatrix, vector)

class Pinhole(object):
    def __init__(self, focalDistance=1.0):
        self.focalDistance = focalDistance
        self.transformMatrix = self.createTransformMatrix(self.focalDistance)

    def createTransformMatrix(self, focalDistance):
        f = focalDistance
        return visual.array([
            [f, 0, 0, 0],
            [0, f, 0, 0],
            [0, 0, f, 0],
            [0, 0, 1, 0]
        ])

    def transform(self, vector):
        return transform.transform(self.transformMatrix, vector)

class LensDemo(object):
    def __init__(self):
        #self.sensorPoint = visual.vector(2,3,4)
        self.aperturePoint = visual.vector(0,0.5,0)
        self.lens = ThinLens(2, 10)
        #self.lens = Pinhole(2)
        self.incoming = spherical.SphericalCoords(0, 0)

        self.initScene()
        
    def update(self,
               #sensorPoint=None, aperturePoint=None,
               incomingTheta=None, incomingPhi=None,
               focalDistance=None, apertureX=None, apertureY=None):
        #if sensorPoint != None:
        #    self.sensorPoint = sensorPoint
        #if aperturePoint != None:
        #    self.aperturePoint = aperturePoint
        if incomingTheta != None:
            self.incoming.theta = incomingTheta * math.pi
        if incomingPhi != None:
            self.incoming.phi = incomingPhi * 2 * math.pi
        if focalDistance != None:
            self.lens.focalDistance = focalDistance
        if apertureX != None:
            self.aperturePoint[0] = apertureX
        if apertureY != None:
            self.aperturePoint[1] = apertureY

        # set incoming arrow so that it point towards the transmission point
        #incomingDir = aperturePoint - sensorPoint
        incomingDir = visual.vector(self.incoming.toCartesian())
        self.incomingArrow.pos = incomingDir + self.aperturePoint
        self.incomingArrow.axis = -incomingDir

        outgoingDir = -self.aperturePoint - self.lens.transform(incomingDir)
        outgoingDir = outgoingDir.norm()
        self.outgoingArrow.pos = self.aperturePoint
        self.outgoingArrow.axis = outgoingDir

    def initScene(self):
        w = 500
        visual.display(x=w, y=0, width=w, height=w, range=1.5, forward=-visual.vector(0,1,1), newzoom=1)
        self.controls = visual.controls.controls(0, 0, width=w, height=w)
        self.incomingThetaSlider = visual.controls.slider(pos=(-20,-50), width=7, length=100, axis=(0,1,0),
            action=lambda: self.update(incomingTheta=(self.incomingThetaSlider.value / 100.0)-0.5))
        self.incomingPhiSlider = visual.controls.slider(pos=(-10,-50), width=7, length=100, axis=(0,1,0),
            action=lambda: self.update(incomingPhi=self.incomingPhiSlider.value / 100.0))
        self.apertureXSlider = visual.controls.slider(pos=(0,-50), width=7, length=100, axis=(0,1,0),
            action=lambda: self.update(apertureX=(self.apertureXSlider.value / 100.0)-0.5))
        self.apertureYSlider = visual.controls.slider(pos=(10,-50), width=7, length=100, axis=(0,1,0),
            action=lambda: self.update(apertureY=(self.apertureYSlider.value / 100.0)-0.5))

        frame.drawWorldCoordinates()
        visual.box(pos=(0,0,0), axis=(0,0,1), length=0.01, height=1.0, width=1.0, opacity=0.25)
        visual.box(pos=(0,0,-0.5), axis=(0,0,1), length=1, height=1.0, width=1.0, opacity=0.1)

        self.incomingArrow = frame.drawRayDirection()
        self.outgoingArrow = frame.drawRayDirection(color=visual.color.orange)

        self.incomingThetaSlider.value = 100.0 * ((self.incoming.theta / math.pi) + 0.5)
        self.incomingPhiSlider.value = 100.0 * self.incoming.phi / (2 * math.pi)
        self.apertureXSlider.value = 100.0 * (self.aperturePoint[0] + 0.5)
        self.apertureYSlider.value = 100.0 * (self.aperturePoint[1] + 0.5)

    def run(self):
        while True:
            visual.rate(100)
            self.controls.interact() # check for events, drive actions; must be executed repeatedly in a loop

demo = LensDemo()
demo.run()
