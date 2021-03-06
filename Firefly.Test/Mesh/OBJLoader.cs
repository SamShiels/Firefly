using Firefly.World.Mesh;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;

namespace Firefly.Test.Mesh
{
  [TestClass]
  public class OBJLoader
  {
    [TestMethod]
    public void LoadOBJ()
    {
      World.Mesh.OBJLoader loader = new World.Mesh.OBJLoader();
      Model plane = loader.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("Firefly.Test.Resources.plane.obj"));

      float[] expected = new float[18]
      {
        -1.0f, -1.0f, 0.0f,
         1.0f, -1.0f, 0.0f,
         1.0f,  1.0f, 0.0f,
        -1.0f, -1.0f, 0.0f,
         1.0f,  1.0f, 0.0f,
        -1.0f,  1.0f, 0.0f
      };

      CollectionAssert.AreEqual(plane.Vertices, expected);
    }
  }
}
