/*
-----------------------------------------------------------------------------
Filename:    TutorialApplication.cpp
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

#include "TutorialApplication.h"

//-------------------------------------------------------------------------------------
TutorialApplication::TutorialApplication(void)
{
}
//-------------------------------------------------------------------------------------
TutorialApplication::~TutorialApplication(void)
{
}

//-------------------------------------------------------------------------------------
void TutorialApplication::createScene(void)
{
    // create your scene here :)
    mSceneMgr->setSkyBox(true, "Examples/SpaceSkyBox");
 
	mSceneMgr->createLight("Light")->setPosition(75,75,75);
 
	Ogre::Entity* cubeEntity = mSceneMgr->createEntity("Cube", "ogrehead.mesh");
	Ogre::SceneNode* cubeNode = mSceneMgr->getRootSceneNode()->createChildSceneNode("CubeNode");
	cubeNode->attachObject(cubeEntity);

}
