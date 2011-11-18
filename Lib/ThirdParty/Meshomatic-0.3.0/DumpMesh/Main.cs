using System;
using Meshomatic;

namespace Meshomatic.DumpMesh {
	class DumpMesh {
		public static void Main(string[] args) {
			// I fucking hate all this fucking pure-fucking-object fucking bullshit.
			//MeshData a = new ObjLoader().LoadFile("test.obj");
			MeshData a = new Ms3dLoader().LoadFile("test.ms3d");
			Console.WriteLine(a);
		}
	}
}
