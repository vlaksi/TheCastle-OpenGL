using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using AssimpSample.Services;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;


namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

        private readonly CoordinateValidator _coordinateValidator;

        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            // Inicijalizacija komponenti
            InitializeComponent();



            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(
                    Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models"),
                    "Arrow.dae",
                    (int)openGLControl.Width,
                    (int)openGLControl.Height, openGLControl.OpenGL);
                _coordinateValidator = new CoordinateValidator();
                //m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Arrow"), "Arrow.dae", (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
        }

        public CoordinateValidator CoordinateValidator
        {
            get { return _coordinateValidator; }
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.V:
                    m_world.AktivacijaAnimacije();
                    ZabranaInterakcije = true;
                    break;
                case Key.P:
                    m_world.DeaktivacijaAnimacije();
                    ZabranaInterakcije = false;
                    break;
                case Key.F4: this.Close(); break;
                case Key.I:
                    if (!ZabranaInterakcije)
                    {
                        if (CoordinateValidator.ValidDownRotate(m_world.RotationX))
                            m_world.RotationX -= 5.0f;
                        else
                            m_world.RotationX -= -5.0f;
                    }
                    break;
                case Key.K:
                    if (!ZabranaInterakcije)
                    {
                        if (CoordinateValidator.ValidUpRotate(m_world.RotationX))
                            m_world.RotationX += 5.0f;
                        else
                            m_world.RotationX += -5.0f;
                    }
                    break;
                case Key.J:
                    if (!ZabranaInterakcije)
                    {
                        if (CoordinateValidator.ValidLeftRotate(m_world.RotationY))
                            m_world.RotationY -= 5.0f;
                        else
                            m_world.RotationY += 5.0f;
                    }
                    break;
                case Key.L:
                    if (!ZabranaInterakcije)
                    {
                        if (CoordinateValidator.ValidRightRotate(m_world.RotationY))
                            m_world.RotationY += 5.0f;
                        else
                            m_world.RotationY -= 5.0f;
                    }
                    break;
                case Key.Add:
                    if (!ZabranaInterakcije)
                    {
                        m_world.SceneDistance -= 700.0f;
                    }
                    break;
                case Key.Subtract:
                    if (!ZabranaInterakcije)
                    {
                        m_world.SceneDistance += 700.0f;
                    }

                    break;
                case Key.F2:
                    OpenFileDialog opfModel = new OpenFileDialog();
                    bool result = (bool)opfModel.ShowDialog();
                    if (result)
                    {

                        try
                        {
                            World newWorld = new World(Directory.GetParent(opfModel.FileName).ToString(), Path.GetFileName(opfModel.FileName), (int)openGLControl.Width, (int)openGLControl.Height, openGLControl.OpenGL);
                            m_world.Dispose();
                            m_world = newWorld;
                            m_world.Initialize(openGLControl.OpenGL);
                        }
                        catch (Exception exp)
                        {
                            MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta:\n" + exp.Message, "GRESKA", MessageBoxButton.OK);
                        }
                    }
                    break;
            }
        }

        public bool ZabranaInterakcije { get; set; }


        #region Rotacija levog zastitnog zida

        private void RotacijaLevogZidaTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            bool negativanBroj = false;
            string ucitaniTekst = rotacijaLevogZidaTextBox.Text;

            if (ucitaniTekst.Length <= 0) return;

            if (ucitaniTekst[0] == '-')
            {
                negativanBroj = true;
                ucitaniTekst = ucitaniTekst.Substring(1, ucitaniTekst.Length - 1);
            }

            if (ucitaniTekst.Length >= 1 & ucitaniTekst.Length <= 3)
            {
                if (!IsInputNumber(ucitaniTekst)) return;
                var vrednostRotacijeLevogZida = ParsirajInput(ucitaniTekst, negativanBroj);
                PostaviVrednostRotacije(vrednostRotacijeLevogZida);
            }
        }

        private void PostaviVrednostRotacije(int vrednostRotacijeLevogZida)
        {
            if (m_world != null)
                m_world.RotacijaLevogZida += vrednostRotacijeLevogZida;
        }

        #endregion


        #region Transliranje desnog zida

        private void TransliranjeDesnogZidaTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            bool negativanBroj = false;
            string ucitaniTekst = transliranjeDesnogZidaTextBox.Text;

            if (ucitaniTekst.Length <= 0) return;

            if (ucitaniTekst[0] == '-')
            {
                negativanBroj = true;
                ucitaniTekst = ucitaniTekst.Substring(1, ucitaniTekst.Length - 1);
            }


            if (ucitaniTekst.Length >= 1 & ucitaniTekst.Length <= 3)
            {
                if (!IsInputNumber(ucitaniTekst)) return;
                var vrednostTransliranjaDesnogZida = ParsirajInput(ucitaniTekst, negativanBroj);
                PostaviVrednostTransliranja(vrednostTransliranjaDesnogZida);
            }
        }

        private void PostaviVrednostTransliranja(int vrednostTransliranjaDesnogZida)
        {
            if (m_world != null)
                m_world.TranslacijaDesnogZida += vrednostTransliranjaDesnogZida;
        }

        private int ParsirajInput(string ucitaniTekst, bool negativanBroj = false)
        {
            int parsiranInput = Convert.ToInt16(ucitaniTekst, 10);
            if (negativanBroj)
                parsiranInput *= -1;
            return parsiranInput;
        }

        private bool IsInputNumber(string ucitaniTekst)
        {
            foreach (char tempChar in ucitaniTekst)
            {
                if (!Char.IsDigit(tempChar))
                    return false;
            }

            return true;
        }

        #endregion


        #region Faktor skaliranja strele

        private void FaktorSkaliranjaStreleTextBox_OnTextChanged(object sender, TextChangedEventArgs e)
        {
            bool negativanBroj = false;
            string ucitaniTekst = faktorSkaliranjaStreleTextBox.Text;

            if (ucitaniTekst.Length <= 0) return;

            if (ucitaniTekst[0] == '-')
            {
                negativanBroj = true;
                ucitaniTekst = ucitaniTekst.Substring(1, ucitaniTekst.Length - 1);
            }


            if (ucitaniTekst.Length >= 1 & ucitaniTekst.Length <= 3)
            {
                if (IsNumberWithDigit(ucitaniTekst))
                {
                    var faktorSkaliranjaSaTackom = float.Parse(ucitaniTekst);
                    PostaviVrednostSkaliranja(faktorSkaliranjaSaTackom);
                }
                else
                {
                    if (!IsInputNumber(ucitaniTekst)) return;
                    var faktorSkaliranja = ParsirajInput(ucitaniTekst, negativanBroj);
                    PostaviVrednostSkaliranja(faktorSkaliranja);
                }
            }
        }

        private bool IsNumberWithDigit(string ucitaniTekst)
        {
            if (ucitaniTekst.Contains('.')) return true;
            return false;
        }

        private void PostaviVrednostSkaliranja(float vrednostFaktoraSKaliranja)
        {
            if (m_world != null)
                m_world.FaktorSkaliranjaStrele += vrednostFaktoraSKaliranja;
        }

        #endregion


    }
}
