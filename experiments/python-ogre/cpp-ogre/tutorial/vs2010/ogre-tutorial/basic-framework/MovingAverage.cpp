#include "stdafx.h"

#include "MovingAverage.h"
 
//-------------------------------------------------------------------------------------
MovingAverage::MovingAverage(void)
{
    mLastCameraOffset = Ogre::Vector3::ZERO;
    totalTime = Ogre::Real(0.0);
}
//-------------------------------------------------------------------------------------
MovingAverage::~MovingAverage(void)
{
}
 
//-------------------------------------------------------------------------------------
void MovingAverage::createScene(void)
{
	mSceneMgr->setAmbientLight(Ogre::ColourValue(0.2f, 0.2f, 0.2f));
 
	Ogre::Light* light = mSceneMgr->createLight("MainLight");
	light->setPosition(20, 80, 50);

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
    
    Ogre::CompositorManager::getSingleton().addCompositor(mWindow->getViewport(0), "DoFAccum");
    Ogre::CompositorManager::getSingleton().setCompositorEnabled(mWindow->getViewport(0), "DoFAccum", true);
    
}

void MovingAverage::setupCompositors(void) {
    // based on Ogre3D Samples\Compositor\src\Compositor.cpp

    Ogre::CompositorPtr compositor = Ogre::CompositorManager::getSingleton().create(
			"DoFAccum", Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME
		);
	Ogre::CompositionTechnique *t = compositor->createTechnique();
	{
		Ogre::CompositionTechnique::TextureDefinition *def = t->createTextureDefinition("currentFrame");
		def->width = 0;
		def->height = 0;
		def->formatList.push_back(Ogre::PF_R8G8B8);
	}
	{
		Ogre::CompositionTechnique::TextureDefinition *def = t->createTextureDefinition("movingAverage");
		def->width = 0;
		def->height = 0;
		def->formatList.push_back(Ogre::PF_R8G8B8);
	}
	{
		Ogre::CompositionTechnique::TextureDefinition *def = t->createTextureDefinition("updatedMovingAverage");
		def->width = 0;
		def->height = 0;
		def->formatList.push_back(Ogre::PF_R8G8B8);
	}
	/// Render scene
	{
		Ogre::CompositionTargetPass *tp = t->createTargetPass();
		tp->setInputMode(Ogre::CompositionTargetPass::IM_PREVIOUS);
		tp->setOutputName("currentFrame");
	}
	/// Initialisation pass for sum texture
	{
        // NOTE: this might not be necessary (?)
		Ogre::CompositionTargetPass *tp = t->createTargetPass();
		tp->setInputMode(Ogre::CompositionTargetPass::IM_PREVIOUS);
		tp->setOutputName("movingAverage");
		tp->setOnlyInitial(true);
	}
	/// Compute updated moving average
	{
		Ogre::CompositionTargetPass *tp = t->createTargetPass();
		tp->setInputMode(Ogre::CompositionTargetPass::IM_NONE);
		tp->setOutputName("updatedMovingAverage");
		{
            Ogre::CompositionPass *pass = tp->createPass();
			pass->setType(Ogre::CompositionPass::PT_RENDERQUAD);
			pass->setMaterialName("BokehLab/DoFAccum/Combine");
			pass->setInput(0, "currentFrame");
			pass->setInput(1, "movingAverage");
		}
	}
	/// Copy back sum texture
	{
		Ogre::CompositionTargetPass *tp = t->createTargetPass();
		tp->setInputMode(Ogre::CompositionTargetPass::IM_NONE);
		tp->setOutputName("movingAverage");
		{
            Ogre::CompositionPass *pass = tp->createPass();
			pass->setType(Ogre::CompositionPass::PT_RENDERQUAD);
			pass->setMaterialName("BokehLab/DoFAccum/Copyback");
			pass->setInput(0, "updatedMovingAverage");
		}
	}
	/// Display result
	{
		Ogre::CompositionTargetPass *tp = t->getOutputTargetPass();
		tp->setInputMode(Ogre::CompositionTargetPass::IM_NONE);
		{
            Ogre::CompositionPass *pass = tp->createPass();
			pass->setType(Ogre::CompositionPass::PT_RENDERQUAD);
			pass->setMaterialName("BokehLab/DoFAccum/Display");
			pass->setInput(0, "movingAverage");
		}
	}
}

void MovingAverage::createCamera(void) {
    BaseApplication::createCamera();

    mCamera->setPosition(Ogre::Vector3(60, 200, 70));
	mCamera->lookAt(0,0,0);

    //mCameraMan->setStyle(OgreBites::CS_MANUAL);
}

bool MovingAverage::frameRenderingQueued(const Ogre::FrameEvent& evt)
{
    // rotate the plane
	//mPlaneNode->yaw(Ogre::Radian(evt.timeSinceLastFrame));

    // perturb the camera position
    totalTime += evt.timeSinceLastFrame;

    Ogre::Real lensRadius = 2.0;
    Ogre::Real angle = totalTime * 10;

    // make offset in the XY plane in the camera space whose normal, -Z, is
    // the camera view direction

    // TODO: the offset should be uniformly sampled from within the lens disk
    // using concentric disk sampling

    mCamera->moveRelative(-mLastCameraOffset);
    mLastCameraOffset = lensRadius * Ogre::Vector3(Ogre::Math::Sin(angle), Ogre::Math::Cos(angle), 0.0);
    mCamera->moveRelative(mLastCameraOffset);

	return BaseApplication::frameRenderingQueued(evt);
}
