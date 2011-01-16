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

    Ogre::Rectangle2D *screenEnt = new Ogre::Rectangle2D(true);
    screenEnt->setCorners(-1.0f, 1.0f, 1.0f, -1.0f);
    screenEnt->setBoundingBox(Ogre::AxisAlignedBox(
        -100000.0f * Ogre::Vector3::UNIT_SCALE,
        100000.0f * Ogre::Vector3::UNIT_SCALE));
    //screenEnt->setMaterial("Examples/Rockwall");
    screenEnt->setMaterial("BokehLab/Plot2DFunction");

    Ogre::SceneNode* screenNode = mSceneMgr->getRootSceneNode()->createChildSceneNode("ScreenNode");
    screenNode->attachObject(screenEnt);
}
