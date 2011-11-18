using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;



namespace Meshomatic {
	public class ObjLoader {
		public MeshData LoadStream(Stream stream) {
			StreamReader reader = new StreamReader(stream);
			List<Vector3> points = new List<Vector3>();
			List<Vector3> normals = new List<Vector3>();
			List<Vector2> texCoords = new List<Vector2>();
			List<Tri> tris = new List<Tri>();
			string line;
			char[] splitChars = { ' ' };
			while((line = reader.ReadLine()) != null) {
				line = line.Trim(splitChars);
				line = line.Replace("  ", " ");

				string[] parameters = line.Split(splitChars);

				switch(parameters[0]) {
				case "p":
					// Point
					break;

				case "v":
					// Vertex
					float x = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
					float y = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);
					float z = float.Parse(parameters[3], CultureInfo.InvariantCulture.NumberFormat);
					points.Add(new Vector3(x, y, z));
					break;

                case "vt":
					// TexCoord
					float u = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
					float v = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);
					texCoords.Add(new Vector2(u, v));
					break;

                case "vn":
					// Normal
					float nx = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
					float ny = float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat);
					float nz = float.Parse(parameters[3], CultureInfo.InvariantCulture.NumberFormat);
					normals.Add(new Vector3(nx, ny, nz));
					break;

                case "f":
					// Face
					tris.AddRange(parseFace(parameters));
					break;
				}
			}
			
Vector3[] p = points.ToArray();
			Vector2[] tc = texCoords.ToArray();
			Vector3[] n = normals.ToArray();
			Tri[] f = tris.ToArray();
			
			// If there are no specified texcoords or normals, we add a dummy one.
			// That way the Points will have something to refer to.
			if(tc.Length == 0) {
				tc = new Vector2[1];
				tc[0] = new Vector2(0, 0);
			}
			if(n.Length == 0) {
				n = new Vector3[1];
				n[0] = new Vector3(1, 0, 0);
			}
				
			return new MeshData(p, n, tc, f);
		}

		public MeshData LoadFile(string file) {
                        // Silly me, using() closes the file automatically.
			using(FileStream s = File.Open(file, FileMode.Open)) {
				return LoadStream(s);
			}
		}
		
		private static Tri[] parseFace(string[] indices) {
			Point[] p = new Point[indices.Length-1];
			for(int i = 0; i < p.Length; i++) {
				p[i] = parsePoint(indices[i+1]);
			}
			return Triangulate(p);
			//return new Face(p);
		}
		
		// Takes an array of points and returns an array of triangles.
		// The points form an arbitrary polygon.
		private static Tri[] Triangulate(Point[] ps) {
			List<Tri> ts = new List<Tri>();
			if(ps.Length < 3) {
				throw new Exception("Invalid shape!  Must have >2 points");
			}
			
			Point lastButOne = ps[1];
			Point lastButTwo = ps[0];
			for(int i = 2; i < ps.Length; i++) {
				Tri t = new Tri(lastButTwo, lastButOne, ps[i]);
				lastButOne = ps[i];
				lastButTwo = ps[i-1];
				ts.Add(t);
			}
			return ts.ToArray();
		}
		
		private static Point parsePoint(string s) {
			char[] splitChars = {'/'};
			string[] parameters = s.Split(splitChars);
			int vert, tex, norm;
			vert = tex = norm = 0;
			vert = int.Parse(parameters[0]) - 1;
			// Texcoords and normals are optional in .obj files
			if(parameters[1] != "") {
				tex = int.Parse(parameters[1]) - 1;
			}
			if(parameters[2] != "") {
				norm = int.Parse(parameters[2]) - 1;
			}
			return new Point(vert, norm, tex);
		}
	}
}
