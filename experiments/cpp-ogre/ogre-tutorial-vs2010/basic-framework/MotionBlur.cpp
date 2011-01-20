#include "stdafx.h"

#include "MotionBlur.h"
 
//-------------------------------------------------------------------------------------
MotionBlur::MotionBlur(void)
{
}
//-------------------------------------------------------------------------------------
MotionBlur::~MotionBlur(void)
{
}
 
//-------------------------------------------------------------------------------------
void MotionBlur::createScene(void)
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


    setupCompositors();
    
    Ogre::CompositorManager::getSingleton().addCompositor(mWindow->getViewport(0), "MotionBlur");
    Ogre::CompositorManager::getSingleton().setCompositorEnabled(mWindow->getViewport(0), "MotionBlur", true);
    
}

void MotionBlur::setupCompositors(void) {
    // based on Ogre3D Samples\Compositor\src\Compositor.cpp

    Ogre::CompositorPtr comp3 = Ogre::CompositorManager::getSingleton().create(
			"MotionBlur", Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME
		);
	{
		Ogre::CompositionTechnique *t = comp3->createTechnique();
		{
			Ogre::CompositionTechnique::TextureDefinition *def = t->createTextureDefinition("scene");
			def->width = 0;
			def->height = 0;
			def->formatList.push_back(Ogre::PF_R8G8B8);
		}
		{
			Ogre::CompositionTechnique::TextureDefinition *def = t->createTextureDefinition("sum");
			def->width = 0;
			def->height = 0;
			def->formatList.push_back(Ogre::PF_R8G8B8);
		}
		{
			Ogre::CompositionTechnique::TextureDefinition *def = t->createTextureDefinition("updatedSum");
			def->width = 0;
			def->height = 0;
			def->formatList.push_back(Ogre::PF_R8G8B8);
		}
		/// Render scene
		{
			Ogre::CompositionTargetPass *tp = t->createTargetPass();
			tp->setInputMode(Ogre::CompositionTargetPass::IM_PREVIOUS);
			tp->setOutputName("scene");
		}
		/// Initialisation pass for sum texture
		{
			Ogre::CompositionTargetPass *tp = t->createTargetPass();
			tp->setInputMode(Ogre::CompositionTargetPass::IM_PREVIOUS);
			tp->setOutputName("sum");
			tp->setOnlyInitial(true);
		}
		/// Do the motion blur
		{
			Ogre::CompositionTargetPass *tp = t->createTargetPass();
			tp->setInputMode(Ogre::CompositionTargetPass::IM_NONE);
			tp->setOutputName("updatedSum");
			{ Ogre::CompositionPass *pass = tp->createPass();
			pass->setType(Ogre::CompositionPass::PT_RENDERQUAD);
			pass->setMaterialName("BokehLab/MotionBlur/Combine");
			pass->setInput(0, "scene");
			pass->setInput(1, "sum");
			}
		}
		/// Copy back sum texture
		{
			Ogre::CompositionTargetPass *tp = t->createTargetPass();
			tp->setInputMode(Ogre::CompositionTargetPass::IM_NONE);
			tp->setOutputName("sum");
			{ Ogre::CompositionPass *pass = tp->createPass();
			pass->setType(Ogre::CompositionPass::PT_RENDERQUAD);
			pass->setMaterialName("BokehLab/MotionBlur/Copyback");
			pass->setInput(0, "updatedSum");
			}
		}
		/// Display result
		{
			Ogre::CompositionTargetPass *tp = t->getOutputTargetPass();
			tp->setInputMode(Ogre::CompositionTargetPass::IM_NONE);
			{ Ogre::CompositionPass *pass = tp->createPass();
			pass->setType(Ogre::CompositionPass::PT_RENDERQUAD);
			pass->setMaterialName("BokehLab/MotionBlur/Display");
			pass->setInput(0, "sum");
			}
		}
	}
}
 
bool MotionBlur::frameRenderingQueued(const Ogre::FrameEvent& evt)
{
	mPlaneNode->yaw(Ogre::Radian(evt.timeSinceLastFrame));
	return BaseApplication::frameRenderingQueued(evt);
}
