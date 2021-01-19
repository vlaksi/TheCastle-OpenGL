using System.Drawing;
using SharpGL;
using SharpGL.SceneGraph.Primitives;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Assets;

namespace AssimpSample.Services
{
    public class WallFactory
    {
        private World _world;

        public WallFactory(World world)
        {
            _world = world;
        }

        public void RenderovanjeZidaDesnoOdDvorca(OpenGL gl, int transliranjeDesnogZida=0)
        {
            gl.PushMatrix();
            gl.Scale(1.0f, 26.0f, 7.0f);
            gl.Translate(25.0f + transliranjeDesnogZida, 0.0f, 1.0f);

            Cube zidDesnoOdDvorca = new Cube();
            zidDesnoOdDvorca.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
        }

        public void RenderovanjeZidaLevoOdDvorca(OpenGL gl, int rotacijaLevogZida=0)
        {
            gl.PushMatrix();
            gl.Rotate(rotacijaLevogZida,0.0f,0.0f,1.0f);
            gl.Scale(1.0f, -26.0f, 7.0f);
            gl.Translate(-25.0f, 0.0f, 1.0f);

            Cube zidLevoOdDvorca = new Cube();
            zidLevoOdDvorca.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
        }

        public void RenderovanjeZidaIzaDvorca(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Scale(25.0f, 1.0f, 7.0f);
            gl.Translate(0.0f, 25.0f, 1.0f);

            Cube zidIzaDvorca = new Cube();
            zidIzaDvorca.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
            gl.PopMatrix();
        }
    }
}