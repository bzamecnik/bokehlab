#include "stdafx.h"

#include "DoFAccumListener.h"

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