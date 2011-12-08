from frame import *
from visual import *
from visual.controls import *
import math

class TiltDemo(object):
    def __init__(self):
        self.normal = vector(0,0,1)
        self.up = vector(0,1,0)
        self.xRot = 0
        self.yRot = 0
        self.tiltMode = "XY"
        self.initScene()

    def updateXYTilt(self, xRot=None, yRot=None):
        if xRot != None:
            self.xRot = xRot
        if yRot != None:
            self.yRot = yRot
        if self.tiltMode == "XY":
            normal = self.normal.rotate(self.xRot * math.pi, (1,0,0))
            normal = normal.rotate(self.yRot * math.pi, (0,1,0))
            up = self.up.rotate(self.xRot * math.pi, (1,0,0))
            up = up.rotate(self.yRot * math.pi, (0,1,0))
        elif self.tiltMode == "ZmodX":
            normal = self.normal.rotate(self.xRot * math.pi, (0,0,1))
            normal = normal.rotate(self.yRot * math.pi, vector(0,1,0).rotate(self.xRot * math.pi, (0,0,1)))
            up = self.up.rotate(self.xRot * math.pi, (0,0,1))
        self.plane.axis = normal
        self.plane.up = up
        self.plane.length = 0.01
        self.normalArrow.axis = normal
        self.upArrow.axis = up

    def setTiltMode(self, tiltMode):
        self.tiltMode = tiltMode

    def initScene(self):
        w = 500
        display(x=w, y=0, width=w, height=w, range=1.5, forward=-vector(0,1,1), newzoom=1)
        self.controls = controls(0, 0, width=w, height=w)
        self.xRotSlider = slider(pos=(-10,-50), width=7, length=100, axis=(0,1,0),
            action=lambda: self.updateXYTilt(xRot=self.xRotSlider.value / 100.0 - 0.5))
        self.yRotSlider = slider(pos=(10,-50), width=7, length=100, axis=(0,1,0),
            action=lambda: self.updateXYTilt(yRot=self.yRotSlider.value / 100.0 - 0.5))
        self.tiltModeMenu = menu(pos=(0,-60,0), height=7, width=25, text='Tilt mode')
        self.tiltModeMenu.items.append(('X, Y', lambda: self.setTiltMode("XY")))
        self.tiltModeMenu.items.append(('Z, modified X', lambda: self.setTiltMode("ZmodX")))

        drawWorldCoordinates()
        self.plane = box(length=0.01, axis=self.normal)

        self.normalArrow = drawRayDirection((0,0,0), self.normal)
        self.upArrow = drawRayDirection((0,0,0), self.up, color=color.green)

        self.xRotSlider.value = (self.xRot + 0.5) * 100.0
        self.yRotSlider.value = (self.yRot + 0.5) * 100.0

    def run(self):
        while True:
            rate(100)
            self.controls.interact() # check for events, drive actions; must be executed repeatedly in a loop

demo = TiltDemo()
demo.run()
