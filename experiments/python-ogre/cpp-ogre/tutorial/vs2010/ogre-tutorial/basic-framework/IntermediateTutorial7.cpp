#include "stdafx.h"

#include "IntermediateTutorial7.h"
 
//-------------------------------------------------------------------------------------
IntermediateTutorial7::IntermediateTutorial7(void)
{
}
//-------------------------------------------------------------------------------------
IntermediateTutorial7::~IntermediateTutorial7(void)
{
}
 
//-------------------------------------------------------------------------------------
void IntermediateTutorial7::createScene(void)
{
	mSceneMgr->setAmbientLight(Ogre::ColourValue(0.2f, 0.2f, 0.2f));
 
	Ogre::Light* light = mSceneMgr->createLight("MainLight");
	light->setPosition(20, 80, 50);
 
	mCamera->setPosition(60, 200, 70);
	mCamera->lookAt(0,0,0);
 

    //// create rotating grass plane

	Ogre::MaterialPtr mat = Ogre::MaterialManager::getSingleton().create("PlaneMat", Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);
	Ogre::TextureUnitState* tuisTexture = mat->getTechnique(0)->getPass(0)->createTextureUnitState("grass_1024.jpg");
 
	mPlane = new Ogre::MovablePlane("Plane");
	mPlane->d = 0;
	mPlane->normal = Ogre::Vector3::UNIT_Y;
 
	Ogre::MeshManager::getSingleton().createPlane("PlaneMesh", Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME, *mPlane, 120, 120, 1, 1, true, 1, 1, 1, Ogre::Vector3::UNIT_Z);
	mPlaneEnt = mSceneMgr->createEntity("PlaneEntity", "PlaneMesh");
	mPlaneEnt->setMaterialName("PlaneMat");
 
	mPlaneNode = mSceneMgr->getRootSceneNode()->createChildSceneNode();
	mPlaneNode->attachObject(mPlaneEnt);

    //// create mini screen using Render-to-texture

    Ogre::TexturePtr rtt_texture = Ogre::TextureManager::getSingleton().createManual(
        "RttTex", Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME,
        Ogre::TEX_TYPE_2D, mWindow->getWidth(), mWindow->getHeight(), 0,
        Ogre::PF_R8G8B8, Ogre::TU_RENDERTARGET);

    Ogre::RenderTexture *renderTexture = rtt_texture->getBuffer()->getRenderTarget();
 
    renderTexture->addViewport(mCamera);
    renderTexture->getViewport(0)->setClearEveryFrame(true);
    renderTexture->getViewport(0)->setBackgroundColour(Ogre::ColourValue::Black);
    renderTexture->getViewport(0)->setOverlaysEnabled(false);

    // store the texture to a file
    // Either this way
    //renderTexture->setAutoUpdated(true);
    // or this way
    //renderTexture->update();
    //renderTexture->writeContentsToFile("start.png");

    // draw the texture into a quad
    mMiniScreen = new Ogre::Rectangle2D(true);
    mMiniScreen->setCorners(0.5f, -0.5f, 1.0f, -1.0f);
    mMiniScreen->setBoundingBox(Ogre::AxisAlignedBox(
        -100000.0f * Ogre::Vector3::UNIT_SCALE,
        100000.0f * Ogre::Vector3::UNIT_SCALE));

    Ogre::SceneNode* miniScreenNode = mSceneMgr->getRootSceneNode()->createChildSceneNode("MiniScreenNode");
    miniScreenNode->attachObject(mMiniScreen);

    Ogre::MaterialPtr renderMaterial = Ogre::MaterialManager::getSingleton().create("RttMat", Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);
    Ogre::Technique* matTechnique = renderMaterial->createTechnique();
    matTechnique->createPass();
    renderMaterial->getTechnique(0)->getPass(0)->setLightingEnabled(false);
    renderMaterial->getTechnique(0)->getPass(0)->createTextureUnitState("RttTex");

    mMiniScreen->setMaterial("RttMat");

    renderTexture->addListener(this);

    
    Ogre::CompositorManager::getSingleton().addCompositor(renderTexture->getViewport(0), "Invert");
    Ogre::CompositorManager::getSingleton().setCompositorEnabled(renderTexture->getViewport(0), "Invert", true);
    
}
 
bool IntermediateTutorial7::frameRenderingQueued(const Ogre::FrameEvent& evt)
{
	mPlaneNode->yaw(Ogre::Radian(evt.timeSinceLastFrame));
	return BaseApplication::frameRenderingQueued(evt);
}

void IntermediateTutorial7::preRenderTargetUpdate(const Ogre::RenderTargetEvent& evt)
{
	mMiniScreen->setVisible(false);
}
 
void IntermediateTutorial7::postRenderTargetUpdate(const Ogre::RenderTargetEvent& evt)
{
	mMiniScreen->setVisible(true);
}