using System;
using System.IO;

namespace Meshomatic{
	public class DirectXLoader {

		public DirectXLoader() {
		}
		
		public MeshData LoadFile(string file) {
			using(FileStream s = File.Open(file, FileMode.Open)) {
				return LoadStream(s);
			}
		}
		public MeshData LoadStream(Stream stream) {
			return null;
		}
		
	}
}
