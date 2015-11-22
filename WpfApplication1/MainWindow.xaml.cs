using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using TagLib;



namespace WpfApplication1
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct WindowCompositionAttributeData
    {
        public WindowCompositionAttribute Attribute;
        public IntPtr Data;
        public int SizeOfData;
    }

    internal enum WindowCompositionAttribute
    {
        // ...
        WCA_ACCENT_POLICY = 19
        // ...
    }

    internal enum AccentState
    {
        ACCENT_DISABLED = 0,
        ACCENT_ENABLE_GRADIENT = 1,
        ACCENT_ENABLE_TRANSPARENTGRADIENT = 2,
        ACCENT_ENABLE_BLURBEHIND = 3,
        ACCENT_INVALID_STATE = 4
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AccentPolicy
    {
        public AccentState AccentState;
        public int AccentFlags;
        public int GradientColor;
        public int AnimationId;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //-------------------------------------------------------
        bool playing = false;
        MediaPlayer mediaPlayer = new MediaPlayer();
        TagLib.File musicFile;
        [DllImport("user32.dll")]
        internal static extern int SetWindowCompositionAttribute(IntPtr hwnd, ref WindowCompositionAttributeData data);
        internal void EnableBlur()
        {
            var windowHelper = new WindowInteropHelper(this);

            var accent = new AccentPolicy();
            var accentStructSize = Marshal.SizeOf(accent);
            accent.AccentState = AccentState.ACCENT_ENABLE_BLURBEHIND;

            var accentPtr = Marshal.AllocHGlobal(accentStructSize);
            Marshal.StructureToPtr(accent, accentPtr, false);

            var data = new WindowCompositionAttributeData();
            data.Attribute = WindowCompositionAttribute.WCA_ACCENT_POLICY;
            data.SizeOfData = accentStructSize;
            data.Data = accentPtr;

            SetWindowCompositionAttribute(windowHelper.Handle, ref data);

            Marshal.FreeHGlobal(accentPtr);
        }
        public MainWindow()
        {
            InitializeComponent();
            Uri iconUri = new Uri("Resources/Mushroom - 1UP.ico", UriKind.RelativeOrAbsolute);

            this.Icon = BitmapFrame.Create(iconUri);
        }

        private void windowLoaded(object sender, RoutedEventArgs e)
        {
            EnableBlur();
        }

        void openFileButtonMouseClick(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                mediaPlayer.Open(new Uri(openFileDialog.FileName));
                mediaPlayer.Play();
                musicFile = TagLib.File.Create(openFileDialog.FileName);
                String title = musicFile.Tag.Title;
                String album = musicFile.Tag.Album;
                String length = musicFile.Properties.Duration.ToString();
                musicinfo.Text = title;
            }
        }

        void playPauseButtonClick(object sender, EventArgs e)
        {
            if (!playing)
            {
                mediaPlayer.Play();
                playPauseButton.Content = "点我暂停喵";
                playing = true;
            }
            else
            {
                mediaPlayer.Pause();
                playPauseButton.Content = "点我播放喵";
                playing = false;
            }
        }
        void closewin(object sender, EventArgs e)
        {
            this.Close();

        }
        void MinimizeWin(object sender, EventArgs e)
        {
            this.WindowState = WindowState.Minimized;

        }
        void keyPressed(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (!playing)
                {
                    mediaPlayer.Play();
                    playPauseButton.Content = "点我暂停喵";
                    playing = true;
                } else
                {
                    mediaPlayer.Pause();
                    playPauseButton.Content = "点我播放喵";
                    playing = false;
                }
            
            }
            else if (e.Key == Key.O)
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == true)
                {
                    mediaPlayer.Open(new Uri(openFileDialog.FileName));
                    mediaPlayer.Play();
                    musicFile = TagLib.File.Create(openFileDialog.FileName);
                    String title = musicFile.Tag.Title;
                    String album = musicFile.Tag.Album;
                    String length = musicFile.Properties.Duration.ToString();
                    musicinfo.Text = title;
                }
            }



        }

        void windowMouseDown(object sender, MouseEventArgs e)
        {
            DragMove();
        }

        //---------------------------------------------------------------
    }
}
