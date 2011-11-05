namespace BokehLab.RayTracing.Lens.Test
{
    using System.Collections.Generic;
    using BokehLab.Math;
    using OpenTK;
    using Xunit;

    public class ThinLensTest
    {
        IEqualityComparer<Ray> rayComparer;

        public ThinLensTest()
        {
            rayComparer = new RayEqualityComparer(1e-10);
        }

        [Fact]
        public void TestTransferStraight()
        {
            // object from object space
            // |object.Z| > |f|
            // ray within aperture

            ThinLens thinLens = new ThinLens(5, 4);
            Vector3d lensPos = new Vector3d(2, 3, 0);
            Vector3d objectPos = new Vector3d(1, 2, 15);
            Ray expectedRay = new Ray(lensPos, new Vector3d(-2.5, -4.0, -7.5));
            Ray outgoingRay = thinLens.Transfer(objectPos, lensPos);

            Assert.NotNull(outgoingRay);
            Assert.Equal(expectedRay, outgoingRay, rayComparer);
        }

        [Fact]
        public void TestTransferRayStraightLensCenter()
        {
            // object from object space
            // |object.Z| > |f|
            // ray goes through the lens center

            ThinLens thinLens = new ThinLens(5, 4);
            Vector3d lensPos = new Vector3d(0, 0, 0);
            Vector3d objectPos = new Vector3d(1, 2, 15);
            Ray expectedRay = new Ray(lensPos, new Vector3d(-0.5, -1.0, -7.5));
            Ray outgoingRay = thinLens.Transfer(objectPos, lensPos);

            Assert.NotNull(outgoingRay);
            Assert.Equal(expectedRay, outgoingRay, rayComparer);
        }

        [Fact]
        public void TestTransferRayReverse()
        {
            // object from image space
            // |object.Z| > |f|
            // ray within aperture

            ThinLens thinLens = new ThinLens(5, 4);
            Vector3d lensPos = new Vector3d(2, 3, 0);
            Vector3d objectPos = new Vector3d(1, 2, -15);
            Ray expectedRay = new Ray(lensPos, new Vector3d(-2.5, -4.0, 7.5));
            Ray outgoingRay = thinLens.Transfer(objectPos, lensPos);

            Assert.NotNull(outgoingRay);
            Assert.Equal(expectedRay, outgoingRay, rayComparer);
        }

        [Fact]
        public void TestTransferRayInfiniteImage()
        {
            // |object.Z| = |f|, so that the image is at infinity in image space

            ThinLens thinLens = new ThinLens(5, 4);
            Vector3d lensPos = new Vector3d(2, 3, 0);
            Vector3d objectPos = new Vector3d(1, 2, 5);
            Ray expectedRay = new Ray(lensPos, new Vector3d(1, 2, 5));
            Ray outgoingRay = thinLens.Transfer(objectPos, lensPos);

            Assert.NotNull(outgoingRay);
            Assert.Equal(expectedRay, outgoingRay, rayComparer);
        }

        [Fact(Skip = "")]
        public void TestTransferRayInfiniteObject()
        {
            // object origin is at infinity

            // should be transformed to focal length at the opposite space

            // NOTE: the objectPos argument of the Transfer(objectPos, lensPos)
            // method would have to be Vector4 instead of Vector3
        }

        [Fact(Skip = "")]
        public void TestTransferRayOutsideAperture()
        {
            // ray outside aperture but on the lens plane

            ThinLens thinLens = new ThinLens(5, 4);
            Vector3d lensPos = new Vector3d(20, 30, 0);
            Vector3d objectPos = new Vector3d(1, 2, 5);
            Ray expectedRay = new Ray(lensPos, new Vector3d(1, 2, 5));
            Ray outgoingRay = thinLens.Transfer(objectPos, lensPos);

            Assert.Null(outgoingRay);
        }

        //[Fact]
        //public void TestTransferRayStraight()
        //{
        //    // object from object space
        //    // |object.Z| > |f|
        //    // ray within aperture

        //    ThinLens thinLens = new ThinLens(5, 4);
        //    Vector3d lensPos = new Vector3d(2, 3, 0);
        //    Vector3d direction = new Vector3d(1, 2, 15);
        //    Vector3d objectPos = lensPos - direction;
        //    Ray incomingRay = new Ray(objectPos, direction);
        //    Ray expectedRay = new Ray(lensPos, new Vector3d(-2.5, -3.5, 7.5));
        //    Ray outgoingRay = thinLens.Transfer(incomingRay);

        //    Assert.NotNull(outgoingRay);
        //    Assert.Equal(expectedRay, outgoingRay, rayComparer);
        //}

        //[Fact]
        //public void TestTransferRayStraightLensCenter() {
        //    // object from object space
        //    // |object.Z| > |f|
        //    // ray goes through the lens center

        //    ThinLens thinLens = new ThinLens(5, 4);
        //    Vector3d lensPos = new Vector3d(0, 0, 0);
        //    Vector3d direction = new Vector3d(1, 2, 15);
        //    Vector3d objectPos = lensPos - direction;
        //    Ray incomingRay = new Ray(objectPos, direction);
        //    Ray expectedRay = new Ray(lensPos, new Vector3d(0.5, 1.0, 7.5));
        //    Ray outgoingRay = thinLens.Transfer(incomingRay);

        //    Assert.NotNull(outgoingRay);
        //    Assert.Equal(expectedRay, outgoingRay, rayComparer);
        //}

        //[Fact]
        //public void TestTransferRayReverse()
        //{
        //    // object from image space
        //    // |object.Z| > |f|
        //    // ray within aperture

        //    ThinLens thinLens = new ThinLens(5, 4);
        //    Vector3d lensPos = new Vector3d(2, 3, 0);
        //    Vector3d direction = new Vector3d(1, 2, -15);
        //    Vector3d objectPos = lensPos - direction;
        //    Ray incomingRay = new Ray(objectPos, direction);
        //    Ray expectedRay = new Ray(lensPos, new Vector3d(-2.5, -3.5, -7.5));
        //    Ray outgoingRay = thinLens.Transfer(incomingRay);

        //    Assert.NotNull(outgoingRay);
        //    Assert.Equal(expectedRay, outgoingRay, rayComparer);
        //}

        //[Fact]
        //public void TestTransferRayInfiniteImage(){
        //    // |object.Z| = |f|, so that the image is at infinity in image space

        //    ThinLens thinLens = new ThinLens(5, 4);
        //    Vector3d lensPos = new Vector3d(2, 3, 0);
        //    Vector3d direction = new Vector3d(1, 2, 5);
        //    Vector3d objectPos = lensPos - direction;
        //    Ray incomingRay = new Ray(objectPos, direction);
        //    Ray expectedRay = new Ray(lensPos, new Vector3d(1, 1, -5));
        //    Ray outgoingRay = thinLens.Transfer(incomingRay);

        //    Assert.NotNull(outgoingRay);
        //    Assert.Equal(expectedRay, outgoingRay, rayComparer);
        //}

        //[Fact(Skip = "")]
        //public void TestTransferRayInfiniteObject()
        //{
        //    // object origin is at infinity

        //    // should be transformed to focal length at the opposite space
        //}

        //[Fact(Skip = "")]
        //public void TestTransferRayOutsideAperture()
        //{
        //    // ray outside aperture but on the lens plane

        //    // should return null
        //}

        //[Fact]
        //public void TestTransferRayOriginAtLensPlane()
        //{
        //    ThinLens thinLens = new ThinLens(5, 4);
        //    Vector3d lensPos = new Vector3d(2, 3, 0);
        //    Vector3d direction = new Vector3d(1, 2, 15);
        //    Ray incomingRay = new Ray(lensPos, direction);
        //    Ray expectedRay = new Ray(lensPos, new Vector3d(-4.0, -5.0, 7.5));
        //    Ray outgoingRay = thinLens.Transfer(incomingRay);

        //    Assert.NotNull(outgoingRay);
        //    Assert.Equal(expectedRay, outgoingRay, rayComparer);
        //}

        //[Fact(Skip = "")]
        //public void TestTransferRayOriginOutsideLensPlane()
        //{
        //}
    }
}
