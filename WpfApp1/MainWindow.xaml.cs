
using System;
using System.Windows;
using Emgu.CV;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private System.Windows.Threading.DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
        private VideoCapture _capture = null;
        private bool _captureInProgress;
        private bool record = false;
        private bool _photo = false;
        private VideoWriter writer = new VideoWriter(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + "mov" + ".mp4", VideoWriter.Fourcc('M', 'P', '4', 'V'), 15F, new System.Drawing.Size(1920, 1080), false);

        public MainWindow()
        {
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(1000.0 / 20.0);
            InitializeComponent();
            CvInvoke.UseOpenCL = false;
            try
            {
                Console.WriteLine("here");
                _capture = new VideoCapture("rtsp://admin:123sakha@192.168.100.109:554/ISAPI/Streaming/Channels/101");
                _capture.ImageGrabbed += ProcessFrame;
            }
            catch (NullReferenceException excpt)
            {
                MessageBox.Show(excpt.Message);
            }
        }
        
        
private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (_capture != null)
            {
                Mat frame = new Mat();
                _capture.Retrieve(frame, 0);
                if (frame != null)
                {
                    Image<Emgu.CV.Structure.Gray, Byte> img = frame.ToImage<Emgu.CV.Structure.Gray, Byte>();
                    try
                    {
                        if (record)
                        {
                            writer.Write(img.Mat);
                        }
                        captureImageBox.Source = helper.ToBitmapSource(img.Bitmap);
                    } catch(Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }

                }
            }
        }
        private void ProcessFrame(object sender, EventArgs arg)
        {
            Mat frame = new Mat();
     
            _capture.Retrieve(frame, 0);
            try
            {
                var timeStamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                var path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\" + "img" + timeStamp + ".bmp";
                if (_photo)
                {
                    frame.ToImage<Emgu.CV.Structure.Gray, Byte>().Bitmap.Save(path);
                }
                _photo = false;
               
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           // captureImageBox.Source = new BitmapImage( new Uri("lol.bmp"));


        }
        private void recordButton_Click(object sender, EventArgs e)
        {
            this.record = !this.record;
            if (this.record)
            {
                RecordButton.Content = "Stop Recording";
            } else
            {
                RecordButton.Content = "Start Recording";
            }
        }
        private void photo(object sender, EventArgs e)
        {
            _photo = true;
        }
        private void captureButton_Click(object sender, EventArgs e)
        {
            dispatcherTimer.Start();
            if (_capture != null)
            {
                if (_captureInProgress)
                {  //stop the capture
                    btnLoadFromFile.Content = "Start Capture";
                    _capture.Pause();

                }
                else
                {
                    //start the capture
                    btnLoadFromFile.Content = "Stop";
                    _capture.Start();
                }

                _captureInProgress = !_captureInProgress;
            }
        }

        void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            writer.Dispose();
        }


    }

}
