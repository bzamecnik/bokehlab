using System;
using System.Collections.Generic;
using System.Text;


namespace Meshomatic {
	/**
	 * <summary>
	 * A class containing all the necessary data for a mesh: Points, normal vectors, UV coordinates,
	 * and indices into each.
	 * Regardless of how the mesh file represents geometry, this is what we load it into,
	 * because this is most similar to how OpenGL represents geometry.
	 * We store data as arrays of vertices, UV coordinates and normals, and then a list of Triangle
	 * structures.  A Triangle is a struct which contains integer offsets into the vertex/normal/texcoord
	 * arrays to define a face.
	 * </summary>
	 */
	// XXX: Sources: http://www.opentk.com/files/ObjMeshLoader.cs, OOGL (MS3D), Icarus (Colladia)
	public class MeshData {
		public Vector3[] Vertices;
		public Vector2[] TexCoords;
		public Vector3[] Normals;
		public Tri[] Tris;
		/// <summary>
		///Creates a new MeshData object 
		/// </summary>
		/// <param name="vert">
		/// A <see cref="Vector3[]"/>
		/// </param>
		/// <param name="norm">
		/// A <see cref="Vector3[]"/>
		/// </param>
		/// <param name="tex">
		/// A <see cref="Vector2[]"/>
		/// </param>
		/// <param name="tri">
		/// A <see cref="Tri[]"/>
		/// </param>
		public MeshData(Vector3[] vert, Vector3[] norm, Vector2[] tex, Tri[] tri) {
			Vertices = vert;
			TexCoords = tex;
			Normals = norm;
			Tris = tri;
			
			Verify();
		}
		/// <summary>
		/// Returns an array containing the coordinates of all the <value>Vertices</value>.
	    /// So {<1,1,1>, <2,2,2>} will turn into {1,1,1,2,2,2}
		/// </summary>
		/// <returns>
		/// A <see cref="System.Double[]"/>
		/// </returns>
		public double[] VertexArray() {
			double[] verts = new double[Vertices.Length*3];
			for(int i = 0; i < Vertices.Length; i++) {
				verts[i*3] = Vertices[i].X;
				verts[i*3+1] = Vertices[i].Y;
				verts[i*3+2] = Vertices[i].Z;
			}
			
			return verts;
		}
		
		/// <summary>
		/// Returns an array containing the coordinates of the <value>Normals<,value>, similar to VertexArray.
		/// </summary>
		/// <returns>
		/// A <see cref="System.Double[]"/>
		/// </returns>
		public double[] NormalArray() {
			double[] norms = new double[Normals.Length * 3];
			for(int i = 0; i < Normals.Length; i++) {
				norms[i * 3] = Normals[i].X;
				norms[i * 3 + 1] = Normals[i].Y;
				norms[i * 3 + 2] = Normals[i].Z;
			}
			
			return norms;
		}
		
		/// <summary>
		/// Returns an array containing the coordinates of the <value>TexCoords<value>, similar to VertexArray. 
		/// </summary>
		/// <returns>
		/// A <see cref="System.Double[]"/>
		/// </returns>
		public double[] TexcoordArray() {
			double[] tcs = new double[TexCoords.Length*2];
			for(int i = 0; i < TexCoords.Length; i++) {
				tcs[i*3] = TexCoords[i].X;
				tcs[i*3+1] = TexCoords[i].Y;
			}
			
			return tcs;
		}

		/*
		public void IndexArrays(out int[] verts, out int[] norms, out int[] texcoords) {
			List<int> v = new List<int>();
			List<int> n = new List<int>();
			List<int> t = new List<int>();
			foreach(Face f in Faces) {
				foreach(Point p in f.Points) {
					v.Add(p.Vertex);
					n.Add(p.Normal);
					t.Add(p.TexCoord);
				}
			}
			verts = v.ToArray();
			norms = n.ToArray();
			texcoords = t.ToArray();
		}
		*/
		
		
		/// <summary>
		///  Turns the Triangles into an array of Points.
		/// </summary>
		/// <returns>
		/// A <see cref="Point[]"/>
		/// </returns>
		protected Point[] Points() {
			List<Point> points = new List<Point>();
			foreach(Tri t in Tris) {
				points.Add(t.P1);
				points.Add(t.P2);
				points.Add(t.P3);
			}
			return points.ToArray();
		}
		// OpenGL's vertex buffers use the same index to refer to vertices, normals and floats,
		// and just duplicate data as necessary.  So, we do the same.
		// XXX: This... may or may not be correct, and is certainly not efficient.
		// But when in doubt, use brute force.
		public void OpenGLArrays(out float[] verts, out float[] norms, out float[] texcoords, out uint[] indices) {
			Point[] points = Points();
			verts = new float[points.Length * 3];
			norms = new float[points.Length * 3];
			texcoords = new float[points.Length * 2];
			indices = new uint[points.Length];
			
			for(uint i = 0; i < points.Length; i++) {
				Point p = points[i];
				verts[i*3] = (float) Vertices[p.Vertex].X;
				verts[i*3+1] = (float) Vertices[p.Vertex].Y;
				verts[i*3+2] = (float) Vertices[p.Vertex].Z;
				
				norms[i*3] = (float) Normals[p.Normal].X;
				norms[i*3+1] = (float) Normals[p.Normal].Y;
				norms[i*3+2] = (float) Normals[p.Normal].Z;
				
				texcoords[i*2] = (float) TexCoords[p.TexCoord].X;
				texcoords[i*2+1] = (float) TexCoords[p.TexCoord].Y;
				
				indices[i] = i;
			}
		}
		
		
		public override string ToString() {
			StringBuilder s = new StringBuilder();
			s.AppendLine("Vertices:");
			foreach(Vector3 v in Vertices) {
				s.AppendLine(v.ToString());
			}
			
			s.AppendLine("Normals:");
			foreach(Vector3 n in Normals) {
				s.AppendLine(n.ToString());
			}
			s.AppendLine("TexCoords:");
			foreach(Vector2 t in TexCoords) {
				s.AppendLine(t.ToString());
			}
			s.AppendLine("Tris:");
			foreach(Tri t in Tris) {
				s.AppendLine(t.ToString());
			}
			return s.ToString();
		}
		
		// XXX: Might technically be incorrect, since a (malformed) file could have vertices
		// that aren't actually in any face.
		// XXX: Don't take the names of the out parameters too literally...
		public void Dimensions(out double width, out double length, out double height) {
			double maxx, minx, maxy, miny, maxz, minz;
			maxx = maxy = maxz = minx = miny = minz = 0;
			foreach(Vector3 vert in Vertices) {
				if(vert.X > maxx) maxx = vert.X;
				if(vert.Y > maxy) maxy = vert.Y;
				if(vert.Z > maxz) maxz = vert.Z;
				if(vert.X < minx) minx = vert.X;
				if(vert.Y < miny) miny = vert.Y;
				if(vert.Z < minz) minz = vert.Z;
			}
			width = maxx - minx;
			length = maxy - miny;
			height = maxz - minz;
		}
		
		/// <summary>
		/// Does some simple sanity checking to make sure that the offsets of the Triangles
	    /// actually refer to real points.  Throws an 
	    /// <exception cref="IndexOutOfRangeException">IndexOutOfRangeException</exception> if not.
		/// </summary>
		/// 
		public void Verify() {
			foreach(Tri t in Tris) {
				foreach(Point p in t.Points()) {
					if(p.Vertex >= Vertices.Length) {
						string message = String.Format("Vertex {0} >= length of vertices {1}", p.Vertex, Vertices.Length);
						throw new IndexOutOfRangeException(message);
					}
					if(p.Normal >= Normals.Length) {
						string message = String.Format("Normal {0} >= number of normals {1}", p.Normal, Normals.Length);
						throw new IndexOutOfRangeException(message);
					}
					if(p.TexCoord > TexCoords.Length) {
						string message = String.Format("TexCoord {0} > number of texcoords {1}", p.TexCoord, TexCoords.Length);
						throw new IndexOutOfRangeException(message);
					}
				}
			}
		}
	}
	
	public struct Vector2 {
		public double X;
		public double Y;
		public Vector2(double x, double y) {
			X = x;
			Y = y;
		}
		
		public override string ToString() {return String.Format("<{0},{1}>", X, Y);}
	}
	
	public struct Vector3 {
		public double X;
		public double Y;
		public double Z;
		public Vector3(double x, double y, double z) {
			X = x;
			Y = y;
			Z = z;
		}
		
		public override string ToString() {return String.Format("<{0},{1},{2}>", X, Y, Z);}
	}
	
	public struct Point {
		public int Vertex;
		public int Normal;
		public int TexCoord;
		
		public Point(int v, int n, int t) {
			Vertex = v;
			Normal = n;
			TexCoord = t;
		}
		
		public override string ToString() {return String.Format("Point: {0},{1},{2}", Vertex, Normal, TexCoord);}
	}
	
	public class Tri {
		public Point P1, P2, P3;
		public Tri() {
			P1 = new Point();
			P2 = new Point();
			P3 = new Point();
		}
		public Tri(Point a, Point b, Point c) {
			P1 = a;
			P2 = b;
			P3 = c;
		}
		
		public Point[] Points() {
			return new Point[3]{P1, P2, P3};
		}
		
		public override string ToString() {return String.Format("Tri: {0}, {1}, {2}", P1, P2, P3);}
	}
}
