from visual import *
from visual.controls import *
from spherical import SphericalCoords
from frame import *
import sys

class TransmissionDemo(object):
    def __init__(self, theta=0.0, phi=0.0, etaIn=1.0, etaOut=1.0):
        self.etaIn = etaIn
        self.etaOut = etaOut
        
        self.incoming = SphericalCoords(theta * math.pi, phi * 2 * math.pi)
        self.reflected = self.incoming.reflect()
        self.refracted = self.incoming.refract(etaIn, etaOut)

        self.initScene()
        
    def update(self, theta=None, phi=None, etaIn=None, etaOut=None):
        if theta != None:
            self.incoming.theta = theta * math.pi
        if phi != None:
            self.incoming.phi = phi * 2 * math.pi
        if etaIn != None:
            self.etaIn = (abs(etaIn) > 0) and etaIn or sys.float_info.epsilon
        if etaOut != None:
            self.etaOut = (abs(etaOut) > 0) and etaOut or sys.float_info.epsilon
        self.reflected = self.incoming.reflect()
        self.refracted = self.incoming.refract(self.etaIn, self.etaOut)

        # set incoming arrow so that it point towards the transmission point
        incomingDir = vector(self.incoming.toCartesian())
        self.incomingArrow.pos = incomingDir
        self.incomingArrow.axis = -vector(incomingDir)
        
        self.reflectedArrow.axis = self.reflected.toCartesian()
        
        self.refractedArrow.axis = self.refracted.toCartesian()

    def initScene(self):
        w = 500
        display(x=w, y=0, width=w, height=w, range=1.5, forward=-vector(0,1,1), newzoom=1)
        self.controls = controls(0, 0, width=w, height=w)
        self.incomingThetaSlider = slider(pos=(-10,-50), width=7, length=100, axis=(0,1,0),
            action=lambda: self.update(theta=(self.incomingThetaSlider.value / 100.0)-0.5))
        self.incomingPhiSlider = slider(pos=(10,-50), width=7, length=100, axis=(0,1,0),
            action=lambda: self.update(phi=self.incomingPhiSlider.value / 100.0))
        self.etaInSlider = slider(pos=(50,-50), width=7, length=100, axis=(0,1,0),
            action=lambda: self.update(etaIn=(self.etaInSlider.value / 100.0)))

        drawWorldCoordinates()
        box(pos=(0,0,0), axis=(0,0,1), length=0.01, height=1.0, width=1.0, opacity=0.25)
        box(pos=(0,0,-0.5), axis=(0,0,1), length=1, height=1.0, width=1.0, opacity=0.1)

        self.incomingArrow = drawRayDirection()
        self.reflectedArrow = drawRayDirection(color=visual.color.green)
        self.refractedArrow = drawRayDirection(color=visual.color.orange)

        self.incomingThetaSlider.value = 100.0 * ((self.incoming.theta / math.pi) + 0.5)
        self.incomingPhiSlider.value = 100.0 * self.incoming.phi / (2 * math.pi)
        self.etaInSlider.value = 100.0 * self.etaIn

    def run(self):
        while True:
            rate(100)
            self.controls.interact() # check for events, drive actions; must be executed repeatedly in a loop

demo = TransmissionDemo(theta=0.25, phi=0.5, etaIn=1/1.5)
demo.run()
