// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using System.Drawing;
using System.Drawing.Imaging;
using Assimp;
using System.IO;
using System.Reflection;
using System.Windows.Threading;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using AssimpSample.Services;
using SharpGL.Enumerations;
using SharpGL.SceneGraph.Cameras;
using Scene = Assimp.Scene;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        // Parametri za animaciju
        public float PomerajStrele { get; set; }
        public float PomerajKamere { get; set; }
        public bool StrelaIzlaziVanZamka { get; set; }
        public bool ZauzetPolozajKamere { get; set; }

        private DispatcherTimer timer1;
        private DispatcherTimer timer2;
        private DispatcherTimer timer3;
        private DispatcherTimer timer4;


        // parametri za interakciju preko wpf kontrola
        public int TranslacijaDesnogZida { get; set; }
        public int RotacijaLevogZida { get; set; }
        public int FaktorSkaliranjaStrele { get; set; }

        /// <summary>
        ///	 Identifikatori tekstura za jednostavniji pristup teksturama
        /// </summary>
        private enum TextureObjects { Brick = 0, Floor, Ceiling, PavedMud, Fence, Grass };
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        /// <summary>
        ///	 Identifikatori OpenGL tekstura
        /// </summary>
        private uint[] m_textures = null;

        /// <summary>
        ///	 Putanje do slika koje se koriste za teksture
        /// </summary>
        private string[] m_textureFiles = {
            "..//..//images//brick.jpg",
            "..//..//images//floor.jpg",
            "..//..//images//ceiling.jpg",
            "..//..//images//pavedMud.jpg",
            "..//..//images//fence.png",
            "..//..//images//grass.jpg",
        };


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
        private float m_sceneDistance = 7000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;

        private readonly WallFactory _wallFactory;

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
            _wallFactory = new WallFactory(this);

            m_textures = new uint[m_textureCount];

            FaktorSkaliranjaStrele = 1;
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode: Init, Draw

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            UkljuciTestiranjeDubine(gl);
            UkljuciSakrivanjeNevidljivihPovrsina(gl);

            PodesiOsvetljenje(gl);
            DefinisiTajmereAnimacija();
            PodesiTeksture(gl);

            m_scene_arrow.LoadScene();
            m_scene_arrow.Initialize();
            m_scene_castle.LoadScene();
            m_scene_castle.Initialize();
        }

        private void PodesiTeksture(OpenGL gl)
        {
            // Predji u rezim rada sa 2D teksturama
            gl.Enable(OpenGL.GL_TEXTURE_2D);

            // Teksture se primenjuju sa parametrom modulate
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

            // Ucitaj slike i kreiraj teksture
            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                    System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);		// Linear Filtering
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
                gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);

                // Posto je kreirana tekstura slika nam vise ne treba
                image.UnlockBits(imageData);
                image.Dispose();
            }
        }


        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            ResetujProjekciju(gl);

            gl.PushMatrix();
            PodesiInteraktivneTransformacije(gl);
            SkalirajCitavuScenu(gl);

            DefinisiKameru(gl);

            IscrtajPodlogu(gl);
            IscrtajStrele(gl, FaktorSkaliranjaStrele);
            IscrtajDvorac(gl);
            IscrtajStazu(gl);
            IscrtajZidove(gl);
            gl.PopMatrix();

            IspisiTekst(gl);
            OznaciKrajIscrtavanja(gl);
        }

        private void OznaciKrajIscrtavanja(OpenGL gl)
        {
            gl.Flush();
        }

        private void DefinisiKameru(OpenGL gl)
        {
            gl.LookAt(0, -10, 5, 0, 0, 0, 0, 0, 1);
        }

        private void SkalirajCitavuScenu(OpenGL gl)
        {
            var koeficijentSkaliranja = 60.0f;
            gl.Scale(koeficijentSkaliranja, koeficijentSkaliranja, koeficijentSkaliranja);
        }

        #endregion

        #region Config metode

        private void DefinisiKomponenteMaterijala(OpenGL gl)
        {
            gl.ColorMaterial(OpenGL.GL_FRONT,
                OpenGL.GL_AMBIENT_AND_DIFFUSE); // pozivom metode glColor se definiše ambijentalna i difuzna komponenta materijala.
        }

        private void UkljuciColorTrackingMehanizam(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
        }

        private void UkljuciSakrivanjeNevidljivihPovrsina(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_CULL_FACE); // (BFC - Back face culling)
        }

        private void UkljuciTestiranjeDubine(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_DEPTH_TEST);
        }

        #endregion

        #region Metode: Resize, Dispose

        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            ResetujProjekciju(gl);
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

        #endregion

        #region Metode animacije

        public void AktivacijaAnimacije()
        {
            timer1.Start();
        }

        public void DeaktivacijaAnimacije()
        {
            timer1.Stop();
            timer2.Stop();
            timer3.Stop();
            timer4.Stop();
        }

        private void DefinisiTajmereAnimacija()
        {
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(20);
            timer1.Tick += new EventHandler(InicijalnaAnimacija);

            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromMilliseconds(30);
            timer2.Tick += new EventHandler(AnimacijaUdaljivanjaKamere);

            timer3 = new DispatcherTimer();
            timer3.Interval = TimeSpan.FromMilliseconds(20);
            timer3.Tick += new EventHandler(UpdateAnimation3);

            timer4 = new DispatcherTimer();
            timer4.Interval = TimeSpan.FromSeconds(3f);
            timer4.Tick += new EventHandler(UpdateAnimation4);
        }

        private void InicijalnaAnimacija(object sender, EventArgs e)
        {
            SceneDistance = 700.0f;
            RotationX = 30.0f;
            RotationY = -180.0f;


            timer1.Stop();
            timer3.Stop();
            timer4.Stop();

            timer2.Start();
        }

        private void AnimacijaUdaljivanjaKamere(object sender, EventArgs e)
        {
            SceneDistance -= 60.0f;
            if (SceneDistance <= -2100.0f)
            {
                timer2.Stop();

                SceneDistance = 3400.0f;
                RotationX = -20.0f;
                RotationY = -360.0f;

                timer3.Start();
                timer4.Start();
            }
        }

        /// <summary>
        /// Definiše pomeraj strele
        /// </summary>
        private void UpdateAnimation3(object sender, EventArgs e)
        {
            if (StrelaIzlaziVanZamka)
                PomerajStrele += 0.5f;
            else
                PomerajStrele -= 1f;
        }

        /// <summary>
        /// Obrće smer pomeranja strele
        /// </summary>
        private void UpdateAnimation4(object sender, EventArgs e)
        {
            if (!StrelaIzlaziVanZamka)
            {
                PomerajStrele = 0f;
            }
            StrelaIzlaziVanZamka = !StrelaIzlaziVanZamka;
        }

        /// <summary>
        ///  Funkcija ograničava vrednost na opseg min - max
        /// </summary>
        public static float Clamp(float value, float min, float max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        #endregion

        #region Metode osvetljenja

        /// <summary>
        /// Podesavanje osvetljenja
        /// </summary>
        private void PodesiOsvetljenje(OpenGL gl)
        {
            float[] global_ambient = new float[] { 0.2f, 0.2f, 0.2f, 1.0f };
            gl.LightModel(OpenGL.GL_LIGHT_MODEL_AMBIENT, global_ambient);

            float[] light0pos = new float[] { 100.0f, 1.0f, -7100.0f, 1.0f };
            float[] light0ambient = new float[] { 0.4f, 0.4f, 0.4f, 1.0f };
            float[] light0diffuse = new float[] { 0.3f, 0.3f, 0.3f, 1.0f };
            float[] light0specular = new float[] { 0.8f, 0.8f, 0.8f, 1.0f };


            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPECULAR, light0specular);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f); //TACKASTI IZVOR


            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);

            // Definisemo belu spekularnu komponentu materijala sa jakim odsjajem
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SPECULAR, light0specular);
            gl.Material(OpenGL.GL_FRONT, OpenGL.GL_SHININESS, 128.0f);

            //Uikljuci color tracking mehanizam
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);

            // Podesi na koje parametre materijala se odnose pozivi glColor funkcije
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            // Ukljuci automatsku normalizaciju nad normalama
            gl.Enable(OpenGL.GL_NORMALIZE);


            gl.ShadeModel(OpenGL.GL_SMOOTH);
        }

        #endregion

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

        #region Metoda iscrtavanja

        private void IspisiTekst(OpenGL gl)
        {

            #region Postavljanje gluOrtho2D
            gl.Viewport(0, 0, m_width, m_height); // TODO: Uraditi preko view-porta definisanje pozicije teksta
            gl.MatrixMode(OpenGL.GL_PROJECTION);
            gl.LoadIdentity();
            gl.Ortho2D(-1.0f, 1.0f, -1.0f, 1.0f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            #endregion

            #region Iscrtavanje teksta
            gl.Color(1.0f, 0.0f, 0.0f);

            gl.PushMatrix();
            gl.Translate(0.4f, -0.75f, 0.0f);
            gl.Scale(0.05f, 0.05f, 0.05f);
            gl.DrawText3D("Verdana", 14f, 1f, 1f, "Predmet:Racunarska grafika");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(0.4f, -0.80f, 0.0f);
            gl.Scale(0.05f, 0.05f, 0.05f);
            gl.DrawText3D("Verdana", 14f, 1f, 0.6f, "Sk.god: 2020/21. ");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(0.4f, -0.85f, 0.0f);
            gl.Scale(0.05f, 0.05f, 0.05f);
            gl.DrawText3D("Verdana", 14f, 1f, 0.6f, "Ime: Vladislav");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(0.4f, -0.90f, 0.0f);
            gl.Scale(0.05f, 0.05f, 0.05f);
            gl.DrawText3D("Verdana", 14f, 1f, 0.6f, "Prezime: Maksimovic");
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Translate(0.4f, -0.95f, 0.0f);
            gl.Scale(0.05f, 0.05f, 0.05f);
            gl.DrawText3D("Verdana", 14f, 1f, 0.6f, "Sifra zad: 3.2");
            gl.PopMatrix();

            #endregion
        }

        private void IscrtajZidove(OpenGL gl)
        {
            gl.Disable(OpenGL.GL_CULL_FACE);

            gl.Color(0.9f, 0.9f, 0.9f, 1.0f);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Fence]);

            _wallFactory.RenderovanjeZidaIzaDvorca(gl);
            _wallFactory.RenderovanjeZidaLevoOdDvorca(gl, RotacijaLevogZida);
            _wallFactory.RenderovanjeZidaDesnoOdDvorca(gl, TranslacijaDesnogZida);

            gl.Enable(OpenGL.GL_CULL_FACE);
        }

        private void IscrtajStazu(OpenGL gl)
        {
            gl.PushMatrix();

            gl.Translate(0.0f, 0.0f, 0.1f);

            var visina = 50.0f;
            var sirina = 2.5f;

            gl.Color(0.98f, 0.95f, 0.94f, 1.0f);


            // Prelazim na matricu teksture kako bih skalirao teksturu
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            var koficijentSkaliranjaTeksture = 5f;
            gl.Scale(koficijentSkaliranjaTeksture, koficijentSkaliranjaTeksture, koficijentSkaliranjaTeksture);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.PavedMud]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);

            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-sirina, -sirina);

            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(-sirina, -visina);

            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(sirina, -visina);

            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(sirina, -sirina);
            gl.End();
            gl.PopMatrix();
            // Vracam se na model view matricu, kako bih dalje objekte scene dirao a ne teksturu
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.PopMatrix();
        }

        private void IscrtajPodlogu(OpenGL gl)
        {
            gl.PushMatrix();

            var koeficijentVelicinePodloge = 5;
            var baznaKordinata = 10.0f;
            gl.Color(0.72f, 1.0f, 0.8f);        // rgb(186,255,205) - ali Color metoda ocekuje vrednosti od 0-1 pa sam skalirao na taj opseg
            gl.FrontFace(OpenGL.GL_CCW);

            // Prelazim na matricu teksture kako bih skalirao teksturu
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            var koficijentSkaliranjaTeksture = 3f;
            gl.Scale(koficijentSkaliranjaTeksture, koficijentSkaliranjaTeksture, koficijentSkaliranjaTeksture);

            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Grass]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);

            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-baznaKordinata * koeficijentVelicinePodloge, -baznaKordinata * koeficijentVelicinePodloge);

            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(baznaKordinata * koeficijentVelicinePodloge, -baznaKordinata * koeficijentVelicinePodloge);

            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(baznaKordinata * koeficijentVelicinePodloge, baznaKordinata * koeficijentVelicinePodloge);

            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-baznaKordinata * koeficijentVelicinePodloge, baznaKordinata * koeficijentVelicinePodloge);
            gl.End();

            gl.PopMatrix();
            // Vracam se na model view matricu, kako bih dalje objekte scene dirao a ne teksturu
            gl.MatrixMode(OpenGL.GL_MODELVIEW);

            gl.PopMatrix();
        }

        private void IscrtajDvorac(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Scale(2f, 2f, 2f);
            m_scene_castle.Draw();
            gl.PopMatrix();
        }

        private void IscrtajStrele(OpenGL gl, int faktorSkaliranjaStrele = 1)
        {
            int brojStrela = 10;
            for (float idxStrele = 1; idxStrele <= brojStrela; idxStrele++) // float kako ne bih gubio decimale pri deljenju
            {
                gl.PushMatrix();
                float jedinstveniPomerajStrele;

                gl.Scale(faktorSkaliranjaStrele * 5f, faktorSkaliranjaStrele * 5.0f, faktorSkaliranjaStrele * 5f); // povecavam malo strelu, jer je dosta mala

                jedinstveniPomerajStrele = PomerajStrele;
                jedinstveniPomerajStrele = Clamp(jedinstveniPomerajStrele, 0f, 3000f);

                gl.Translate(0.0f, -jedinstveniPomerajStrele, idxStrele / 3);
                gl.Rotate(90.0f, 1.0f, 0.0f, 0.0f);

                m_scene_arrow.Draw();

                gl.PopMatrix();
            }
        }


        /// <summary>
        /// Interaktivne transformacije u smislu da primenom w,a,s,d kao i +,- kontrola mozemo interagovati s nasom scenom
        /// </summary>
        /// <param name="gl"></param>
        private void PodesiInteraktivneTransformacije(OpenGL gl)
        {
            gl.Translate(0.0f, 1.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);
        }

        private void ResetujProjekciju(OpenGL gl)
        {
            gl.Viewport(0, 0, m_width, m_height); // kreiranje viewport-a po celom prozoru
            gl.MatrixMode(OpenGL.GL_PROJECTION); // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(60f, (double)m_width / m_height, 1f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW); // prebacujemo se da transformisemo matricu nad nasim modelima
            gl.LoadIdentity(); // resetuj ModelView Matrix
        }

        #endregion
    }
}
