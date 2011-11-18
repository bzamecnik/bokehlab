using System;
using System.IO;
using System.Text;
using System.Collections.Generic;


namespace Meshomatic {


	public class Ms3dLoader {
		public MeshData LoadStream(Stream stream) {
			BinaryReader r = new BinaryReader(stream);
			ReadHeader(r);
			Vector3[] verts = ReadVertices(r);
			Ms3dTri[] triangles = ReadMs3dTris(r);
			// There's more data in the stream, but none of it's geometry so we don't read it.
			
			Tri[] t;
			Vector3[] norms;
			Vector2[] texcoords;
			Ms3dTris2Tris(triangles, out t, out norms, out texcoords);
			return new MeshData(verts, norms, texcoords, t);
		}
		
		public MeshData LoadFile(string file) {
			using(FileStream s = File.Open(file, FileMode.Open)) {
				MeshData mesh = LoadStream(s);
				s.Close();
				return mesh;
			}
		}
		
		public MeshData LoadByteArray(byte[] data) {
			using(MemoryStream s = new MemoryStream(data)) {
				MeshData mesh = LoadStream(s);
				s.Close();
				return mesh;
			}
		}
		// Array compare
        // Compare by value, not by reference!
        // Array.Equals is not good enough.
		// XXX: Check IComparable
		private bool thisShouldExistAlready(byte[] a, byte[] b) {
			if(a.Length != b.Length) return false;
			for(int i = 0; i < a.Length; i++) {
				if(a[i] != b[i]) return false;
			}
			return true;
		}
		
		private void ReadHeader(BinaryReader r) {
			byte[] header = r.ReadBytes(10);
			ASCIIEncoding e = new ASCIIEncoding();
			byte[] desiredHeader = e.GetBytes("MS3D000000");
			Int32 version = r.ReadInt32();
			if(!thisShouldExistAlready(header, desiredHeader)) {
				Console.WriteLine(e.GetChars(header));
				Console.WriteLine(e.GetChars(desiredHeader));
				Error("Unknown data type!");
			}
			if(version != 4) {
				Error("Bad MS3D version!  Use version 4.");
			}
			
		}
		
		private Vector3[] ReadVertices(BinaryReader r) {
			UInt16 numVerts = r.ReadUInt16();
			Vector3[] verts = new Vector3[numVerts];
			for(int i = 0; i < numVerts; i++) {
				verts[i] = ReadVertex(r);
			}
			return verts;
		}
		
		private Vector3 ReadVertex(BinaryReader r) {
			r.ReadByte();  // Ignore flags
			float x = r.ReadSingle();
			float y = r.ReadSingle();
			float z = r.ReadSingle();
			r.ReadByte();  // Ignore boneId
			r.ReadByte();  // Ignore referenceCount
			
			return new Vector3((double) x, (double) y, (double) z);
		}
		
		private Ms3dTri[] ReadMs3dTris(BinaryReader r) {
			UInt16 numTris = r.ReadUInt16();
			Ms3dTri[] tris = new Ms3dTri[numTris];
			for(int i = 0; i < numTris; i++) {
				tris[i] = readMs3dTri(r);
			}
			return tris;
		}
		
		private Ms3dTri readMs3dTri(BinaryReader r) {
			Ms3dTri t = new Ms3dTri();
			r.ReadUInt16(); // Ignore flags
			// Vertex indices
			t.Verts[0] = (int) r.ReadUInt16();
			t.Verts[1] = (int) r.ReadUInt16();
			t.Verts[2] = (int) r.ReadUInt16();
			
			// Vertex normals
			t.Normals[0].X = (double) r.ReadSingle();
			t.Normals[0].Y = (double) r.ReadSingle();
			t.Normals[0].Z = (double) r.ReadSingle();
			
			t.Normals[1].X = (double) r.ReadSingle();
			t.Normals[1].Y = (double) r.ReadSingle();
			t.Normals[1].Z = (double) r.ReadSingle();
			
			t.Normals[2].X = (double) r.ReadSingle();
			t.Normals[2].Y = (double) r.ReadSingle();
			t.Normals[2].Z = (double) r.ReadSingle();
			
			// Vertex texcoords
			t.TexCoords[0].X = (double) r.ReadSingle();
			t.TexCoords[1].X = (double) r.ReadSingle();
			t.TexCoords[2].X = (double) r.ReadSingle();
			t.TexCoords[0].Y = 1 - (double) r.ReadSingle();
			t.TexCoords[1].Y = 1 - (double) r.ReadSingle();
			t.TexCoords[2].Y = 1 - (double) r.ReadSingle();
			
			r.ReadByte(); // Ignore smoothingGroup
			r.ReadByte(); // Ignore groupIndex
			
			return t;
		}
		
		// Data munging for fun and profit!
		private void Ms3dTris2Tris(Ms3dTri[] tris, out Tri[] t, out Vector3[] norms, out Vector2[] texs) {
			Tri[] ts = new Tri[tris.Length];
			// Utter utter BS.
			for(int i = 0; i < ts.Length; i++) {
				ts[i] = new Tri();
			}
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> texcoords = new List<Vector2>();
			
			for(int i = 0; i < tris.Length; i++) {
				ts[i].P1.Vertex = tris[i].Verts[0];
				ts[i].P2.Vertex = tris[i].Verts[1];
				ts[i].P3.Vertex = tris[i].Verts[2];
				
				normals.Add(tris[i].Normals[0]);
				normals.Add(tris[i].Normals[1]);
				normals.Add(tris[i].Normals[2]);
				
				ts[i].P1.Normal = 3*i;
				ts[i].P2.Normal = 3*i+1;
				ts[i].P3.Normal = 3*i+2;
				
				texcoords.Add(tris[i].TexCoords[0]);
				texcoords.Add(tris[i].TexCoords[1]);
				texcoords.Add(tris[i].TexCoords[2]);
				
				ts[i].P1.TexCoord = 3*i;
				ts[i].P2.TexCoord = 3*i+1;
				ts[i].P3.TexCoord = 3*i+2;
			}
			
			t = ts;
			norms = normals.ToArray();
			texs = texcoords.ToArray();
		}
		
		// XXX: better exception type?
		private void Error(string s) {
			throw new Exception("Milkshape 3D file loader error: " + s);
		}
		
		
	}
	
	// Pro tip, this is not the same as a Tri.  This is how the ms3d file represents it; it gets converted
	// into Tri's later.
	sealed class Ms3dTri {
		public int[] Verts;
		public Vector3[] Normals;
		public Vector2[] TexCoords;
		
		public Ms3dTri() {
			Verts =  new int[3];
			Normals = new Vector3[3];
			TexCoords = new Vector2[3];
		}
		
	}
	
	
}
