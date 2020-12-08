// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene_arrow;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene_castle;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 100.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene SceneArrow
        {
            get { return m_scene_arrow; }
            set { m_scene_arrow = value; }
        }
        //m_scene_castle

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene SceneCastle
        {
            get { return m_scene_castle; }
            set { m_scene_castle = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {

            // Inicijalizacija scene za arrow i castle: OVDE SAM ISKULIRAO STA SE PROSLEDI IZ MAIN-A ZA PUTANJE, JER MORAM DVE SCENE TJ DVA MODELA UCITATI
            var scenePathForArrow = scenePath + "\\Arrow"; 
            sceneFileName = "Arrow.dae";
            this.m_scene_arrow = new AssimpScene(scenePathForArrow, sceneFileName, gl);

            var scenePathForCastle = scenePath + "\\Castle"; 
            sceneFileName = "CastleModel.obj";
            this.m_scene_castle = new AssimpScene(scenePathForCastle, sceneFileName, gl);

            this.m_width = width;
            this.m_height = height;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            // Model sencenja na flat (konstantno)
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_DEPTH_TEST);    // ukljucujemo testiranje dubine
            gl.Enable(OpenGL.GL_CULL_FACE);     // ukljucujem sakrivanje nevidljivih povrsina (BFC - Back face culling)
            m_scene_arrow.LoadScene();
            m_scene_arrow.Initialize();
            m_scene_castle.LoadScene();
            m_scene_castle.Initialize();
        }

        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            KreiranjePodloge(gl);
            ManipulacijaStrelom(gl);
            ManipulacijaDvorcem(gl);

            // Oznaci kraj iscrtavanja
            gl.Flush();
        }

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0,0,m_width,m_height);      // kreiranje viewport-a po celom prozoru
            gl.MatrixMode(OpenGL.GL_PROJECTION);        // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(60f, (double)width / height, 1f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                          // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene_arrow.Dispose();
            }
        }

        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode

        #region Moje pomocne metode

        private void KreiranjePodloge(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);

            var koeficijentVelicinePodloge = 5;
            var baznaKordinata = 10.0f;
            gl.Color(0.72f, 1.0f, 0.8f);        // rgb(186,255,205) - ali Color metoda ocekuje vrednosti od 0-1 pa sam skalirao na taj opseg
            gl.Begin(OpenGL.GL_QUADS);
            gl.Vertex(-baznaKordinata * koeficijentVelicinePodloge, -baznaKordinata * koeficijentVelicinePodloge);
            gl.Vertex(baznaKordinata * koeficijentVelicinePodloge, -baznaKordinata * koeficijentVelicinePodloge);
            gl.Vertex(baznaKordinata * koeficijentVelicinePodloge, baznaKordinata * koeficijentVelicinePodloge);
            gl.Vertex(-baznaKordinata * koeficijentVelicinePodloge, baznaKordinata * koeficijentVelicinePodloge);
            gl.End();

            gl.PopMatrix();
        }

        private void ManipulacijaDvorcem(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.Scale(2f, 2f, 2f); 
            m_scene_castle.Draw();
            gl.PopMatrix();
        }

        private void ManipulacijaStrelom(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(10.0f, 10.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
            gl.Scale(5f, 5f, 5f); // povecavam malo strelu, jer je dosta mala
            m_scene_arrow.Draw();
            gl.PopMatrix();
        }

        #endregion
    }
}
