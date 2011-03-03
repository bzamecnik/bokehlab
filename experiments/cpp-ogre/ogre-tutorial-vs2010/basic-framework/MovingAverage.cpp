#include "stdafx.h"

#include "MovingAverage.h"
 
//-------------------------------------------------------------------------------------
MovingAverage::MovingAverage(void)
{
    mLastCameraOffset = Ogre::Vector3::ZERO;
    mCurrentFrameOffset = Ogre::Vector2::ZERO;
    mLensRadius = 5.0;
    mFocusDistance = 170.0;
    resetCurrentFrameIndex();
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

    mSceneMgr->setSkyBox(true, "Examples/SpaceSkyBox");
 
    for (int i = 0; i < 5; i++) {
	    Ogre::Entity* ogreHead = mSceneMgr->createEntity("OgreHead" + i, "ogrehead.mesh");
	    Ogre::SceneNode* ogreNode = mSceneMgr->getRootSceneNode()->createChildSceneNode("OgreHead" + i, Ogre::Vector3( i * 100, 0, 0 ));
	    ogreNode->attachObject(ogreHead);
    }

    //// create a grass plane

	//Ogre::MaterialPtr mat = Ogre::MaterialManager::getSingleton().create("PlaneMat", Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME);
	//Ogre::TextureUnitState* tuisTexture = mat->getTechnique(0)->getPass(0)->createTextureUnitState("grass_1024.jpg");
 //
	//mPlane = new Ogre::MovablePlane("Plane");
	//mPlane->d = 0;
	//mPlane->normal = Ogre::Vector3::UNIT_Y;
 //
	//Ogre::MeshManager::getSingleton().createPlane("PlaneMesh", Ogre::ResourceGroupManager::DEFAULT_RESOURCE_GROUP_NAME, *mPlane, 120, 120, 1, 1, true, 1, 1, 1, Ogre::Vector3::UNIT_Z);
	//mPlaneEnt = mSceneMgr->createEntity("PlaneEntity", "PlaneMesh");
	//mPlaneEnt->setMaterialName("PlaneMat");
 //
	//mPlaneNode = mSceneMgr->getRootSceneNode()->createChildSceneNode();
	//mPlaneNode->attachObject(mPlaneEnt);


    setupCompositors();
    
    Ogre::CompositorManager::getSingleton().addCompositor(mWindow->getViewport(0), "DoFAccum");
    Ogre::CompositorManager::getSingleton().setCompositorEnabled(mWindow->getViewport(0), "DoFAccum", true);

    Ogre::CompositorInstance* compositor = Ogre::CompositorManager::getSingleton().getCompositorChain(
        mWindow->getViewport(0))->getCompositor("DoFAccum");
    compositor->addListener(new DoFAccumListener(*this));
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
        def->formatList.push_back(Ogre::PF_FLOAT32_RGB);
	}
	{
		Ogre::CompositionTechnique::TextureDefinition *def = t->createTextureDefinition("updatedMovingAverage");
		def->width = 0;
		def->height = 0;
		def->formatList.push_back(Ogre::PF_FLOAT32_RGB);
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
            pass->setIdentifier(0xDEADBABE);
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

    //mCamera->setPosition(Ogre::Vector3(60, 200, 70));
    mCamera->setPosition(Ogre::Vector3(-70, 0, 60));
	mCamera->lookAt(30,5,0);

    //mCameraMan->setStyle(OgreBites::CS_MANUAL);
}

bool MovingAverage::keyPressed( const OIS::KeyEvent &arg )
{
    if (arg.key == OIS::KC_P) // increase lens aperture
    {
        mLensRadius += 0.5;
    } else if (arg.key == OIS::KC_L) // decrease lens aperture
    {
        mLensRadius -= 0.5;
        mLensRadius = (mLensRadius > 0.0) ? mLensRadius : 0.0;
    } else if (arg.key == OIS::KC_O) // increase focus distance
    {
        mFocusDistance /= 0.9;
    } else if (arg.key == OIS::KC_K) // decrease focus distance
    {
        mFocusDistance *= 0.9;
    }
    resetCurrentFrameIndex();
    return BaseApplication::keyPressed(arg);
}

bool MovingAverage::keyReleased( const OIS::KeyEvent &arg )
{
    resetCurrentFrameIndex();
    return BaseApplication::keyReleased(arg);
}

bool MovingAverage::mouseMoved( const OIS::MouseEvent &arg )
{
    resetCurrentFrameIndex();
    return BaseApplication::mouseMoved(arg);
}

bool MovingAverage::mousePressed( const OIS::MouseEvent &arg, OIS::MouseButtonID id )
{
    resetCurrentFrameIndex();
    return BaseApplication::mousePressed(arg, id);
}

bool MovingAverage::mouseReleased( const OIS::MouseEvent &arg, OIS::MouseButtonID id )
{
    resetCurrentFrameIndex();
    return BaseApplication::mouseReleased(arg, id);
}

void MovingAverage::resetCurrentFrameIndex() {
    mCurrentFrameIndex = 1;
}

Ogre::Vector2 MovingAverage::getCurrentFrameOffset() const {
    Ogre::Real left, right, top, bottom;
    mCamera->getFrustumExtents(left, right, top, bottom);
    return Ogre::Vector2(
        mCurrentFrameOffset.x / (right - left),
        mCurrentFrameOffset.y / (top - bottom));

    //Ogre::Vector3 offsetAtNearPlane(mCurrentFrameOffset.x, mCurrentFrameOffset.y, mCamera->getNearClipDistance());
    //Ogre::Vector3 offsetInScreenSpace = mCamera->getProjectionMatrix() * offsetAtNearPlane;
    //return Ogre::Vector2(offsetInScreenSpace.x, offsetInScreenSpace.y);

    // return OgreBites::SdkTrayManager::sceneToScreen(mCamera, offsetAtNearPlane);
}

bool MovingAverage::frameRenderingQueued(const Ogre::FrameEvent& evt)
{


    // make offset in the XY plane in the camera space whose normal, -Z, is
    // the camera view direction

    mCamera->moveRelative(-mLastCameraOffset);

    Ogre::Vector2 offset;    
    Ogre::Vector2 randomSquareSamples(Ogre::Math::RangeRandom(0.0, 1.0), Ogre::Math::RangeRandom(0.0, 1.0));
    // the offset is uniformly sampled from within the lens disk
    // using uniform disk sampling
    uniformSampleDisk(randomSquareSamples, &offset);

    offset *= mLensRadius;
    mLastCameraOffset = Ogre::Vector3(offset.x, offset.y, 0.0);
    
    mCamera->moveRelative(mLastCameraOffset);

    // the image in the near has to be translated based on the translation
    // of the camera in the camera plane
    // NOTE: the Y direction in the world space and in the texture space is swapped!
    mCurrentFrameOffset = -(mCamera->getNearClipDistance() / mFocusDistance) * offset * Ogre::Vector2(1, -1);

    ++mCurrentFrameIndex;

    return BaseApplication::frameRenderingQueued(evt);
}

// Transforms uniform samples of unit square [0; 1] x [0; 1] to uniform samples
// of a unit disk.
//
// randomNumbers two uniform random numbers from [0; 1] interval
// diskSamples output parameter, assumed to be already allocated
void MovingAverage::uniformSampleDisk(const Ogre::Vector2& randomNumbers, Ogre::Vector2* diskSamples) {
    Ogre::Real radius = Ogre::Math::Sqrt(randomNumbers.x);
    Ogre::Real theta = 2.0 * Ogre::Math::PI * randomNumbers.y;
    diskSamples->x = radius * Ogre::Math::Cos(theta);
    diskSamples->y = radius * Ogre::Math::Sin(theta);
}

void DoFAccumListener::notifyMaterialSetup(Ogre::uint32 pass_id, Ogre::MaterialPtr &mat)
{
    if(pass_id == 0xDEADBABE)
    {
        fpParams = mat->getTechnique(0)->getPass(0)->getFragmentProgramParameters();
    }
}

void DoFAccumListener::notifyMaterialRender(Ogre::uint32 pass_id, Ogre::MaterialPtr &mat)
{
    if(pass_id == 0xDEADBABE)
    {
        Ogre::Real currentFrameWeight = 1.0 / (Ogre::Real)movingAverageApp.getCurrentFrameIndex();
        fpParams->setNamedConstant("currentFrameWeight", Ogre::Vector4(currentFrameWeight, 0, 0, 0));
        Ogre::Vector2 currentFrameOffset = movingAverageApp.getCurrentFrameOffset();
        fpParams->setNamedConstant("offset", Ogre::Vector4(currentFrameOffset.x, currentFrameOffset.y, 0, 0));
    }
}

