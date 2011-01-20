/*
-----------------------------------------------------------------------------
Filename:    FullscreenQuad.cpp
-----------------------------------------------------------------------------

This source file is part of the
   ___                 __    __ _ _    _ 
  /___\__ _ _ __ ___  / / /\ \ (_) | _(_)
 //  // _` | '__/ _ \ \ \/  \/ / | |/ / |
/ \_// (_| | | |  __/  \  /\  /| |   <| |
\___/ \__, |_|  \___|   \/  \/ |_|_|\_\_|
      |___/                              
      Tutorial Framework
      http://www.ogre3d.org/tikiwiki/
-----------------------------------------------------------------------------
*/

#include "stdafx.h"

#include "FullscreenQuad.h"

//-------------------------------------------------------------------------------------
FullscreenQuad::FullscreenQuad(void)
{
}
//-------------------------------------------------------------------------------------
FullscreenQuad::~FullscreenQuad(void)
{
}

//-------------------------------------------------------------------------------------
void FullscreenQuad::createScene(void)
{
    // Create a full rectangle with stays even when the camera moves
    // and fill it with a texture.

    mScreenEnt = new Ogre::Rectangle2D(true);
    mScreenEnt->setCorners(-1.0f, 1.0f, 1.0f, -1.0f);
    mScreenEnt->setBoundingBox(Ogre::AxisAlignedBox(
        -100000.0f * Ogre::Vector3::UNIT_SCALE,
        100000.0f * Ogre::Vector3::UNIT_SCALE));
    //mScreenEnt->setMaterial("Examples/Rockwall");
    //mScreenEnt->setMaterial("BokehLab/Plot2DFunction");
    //mScreenEnt->setMaterial("BokehLab/UniformMaterialParameter");
    mScreenEnt->setMaterial("BokehLab/UniformCustomParameter");
    colorParameter = Ogre::ColourValue(0.5, 0.7, 0.2);
    mScreenEnt->setCustomParameter(1, colorToVector4(colorParameter));

    Ogre::SceneNode* screenNode = mSceneMgr->getRootSceneNode()->createChildSceneNode("ScreenNode");
    screenNode->attachObject(mScreenEnt);
}

bool FullscreenQuad::frameRenderingQueued(const Ogre::FrameEvent& evt)
{
    // animate hue of the color parameter
    Ogre::Real hue, saturation, brightness;
    colorParameter.getHSB(&hue, &saturation, &brightness);
    hue += evt.timeSinceLastFrame / 10;
    colorParameter.setHSB(hue, saturation, brightness);

	mScreenEnt->setCustomParameter(1, colorToVector4(colorParameter));

	return BaseApplication::frameRenderingQueued(evt);
}

Ogre::Vector4 FullscreenQuad::colorToVector4(const Ogre::ColourValue& color) {
    return Ogre::Vector4(color.r, color.g, color.b, color.a);
}