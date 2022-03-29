//Specify all the using statements which give us the access to all the APIs that we'll need
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace HeartFailure
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        // All the required fields declaration
        private StorageFile selectedStorageFile;
        private string label = "";
        private string httpResponseBody = "";

        public enum Labels
        {
            HeartFailure,
            Non_HeartFailure,
        }

        public MainPage()
        {
            //launchPython();
            this.InitializeComponent();
        }

        // Waiting for a click event to select a file 
        private async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (!await getImage())
            {
                return;
            }
            // After the click event happened and an input selected, we begin the model execution. 
            // Bind the model input
            await imageBind();

            // The URI to launch
            await launchURI();

            // Extract the results
            extractResult();
            // Display the results  
            await displayResult();
        }

        // Use command prompt to launch python flask file
        private async Task launchPython()
        {
            try
            {
                ProcessStartInfo start = new ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = "C:\\Users\\AAEON\\Desktop\\HeartFailure\\HeartFailure_flask\\ONNX_classify_flask.py",
                    //Arguments = "C:\\Users\\AAEON\\Desktop\\aaeon_julian\\a.py",
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };

                using (Process process = Process.Start(start))
                {

                    if (process != null)
                        using (StreamReader reader = process.StandardOutput)
                        {
                            string result = reader.ReadToEnd();
                            System.Diagnostics.Debug.WriteLine(reader);
                        }
                }
                //string args = "C:\\Users\\AAEON\\Desktop\\HeartFailure\\HeartFailure_flask\\ONNX_classify_flask.py";
                //Process.Start("python.exe", args);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }
        }

        // A method to select an input image file
        private async Task<bool> getImage()
        {
            try
            {
                // Trigger file picker to select an image file
                FileOpenPicker fileOpenPicker = new FileOpenPicker();
                fileOpenPicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
                fileOpenPicker.FileTypeFilter.Add(".jpg");
                fileOpenPicker.FileTypeFilter.Add(".png");
                fileOpenPicker.ViewMode = PickerViewMode.Thumbnail;
                selectedStorageFile = await fileOpenPicker.PickSingleFileAsync();
                if (selectedStorageFile == null)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        // A method to convert and bind the input image.   
        private async Task imageBind()
        {
            UIPreviewImage.Source = null;

            try
            {
                SoftwareBitmap softwareBitmap;

                using (IRandomAccessStream stream = await selectedStorageFile.OpenAsync(FileAccessMode.Read))
                {

                    // Create the decoder from the stream 
                    BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);

                    // Get the SoftwareBitmap representation of the file in BGRA8 format
                    softwareBitmap = await decoder.GetSoftwareBitmapAsync();
                    softwareBitmap = SoftwareBitmap.Convert(softwareBitmap, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Premultiplied);
                }

                // Display the image
                SoftwareBitmapSource imageSource = new SoftwareBitmapSource();
                await imageSource.SetBitmapAsync(softwareBitmap);
                UIPreviewImage.Source = imageSource;
            }
            catch (Exception e)
            {
            }
        }

        //Use request to get model predict result
        private async Task launchURI()
        {
            //Create an HTTP client object
            Windows.Web.Http.HttpClient httpClient = new Windows.Web.Http.HttpClient();

            //Add a user-agent header to the GET request. 
            var headers = httpClient.DefaultRequestHeaders;

            //The safe way to add a header value is to use the TryParseAdd method and verify the return value is true,
            //especially if the header value is coming from user input.
            string header = "ie";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            header = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";
            if (!headers.UserAgent.TryParseAdd(header))
            {
                throw new Exception("Invalid header value: " + header);
            }

            // The URI to launch
            string imagePath = selectedStorageFile.Path;
            string Uri = "http://127.0.0.1:5000/?path=" + imagePath;
            //string Uri = "http://127.0.0.1:5000/openvino?path=" + imagePath;
            Uri requestUri = new Uri(Uri);

            //Send the GET request asynchronously and retrieve the response as a string.
            Windows.Web.Http.HttpResponseMessage httpResponse = new Windows.Web.Http.HttpResponseMessage();

            try
            {
                //Send the GET request
                httpResponse = await httpClient.GetAsync(requestUri);
                httpResponse.EnsureSuccessStatusCode();
                httpResponseBody = await httpResponse.Content.ReadAsStringAsync();
            }
            catch (Exception ex)
            {
                httpResponseBody = "Error: " + ex.HResult.ToString("X") + " Message: " + ex.Message;
            }
        }

        // A method to extract the output from the the model 
        private void extractResult()
        {
            // Retrieve the results(httpResponseBody) from flask
            int result = Int32.Parse(httpResponseBody);
            int index = -1;
            if(result == 0)
            {
                index = 0;
            }
            else if(result == 1)
            {
                index = 1;
            }
            else
            {
                index = -1;
                System.Diagnostics.Debug.WriteLine("error");
            }
            label = ((Labels)index).ToString();
            System.Diagnostics.Debug.WriteLine(label);
        }

        // A method to display the result
        private async Task displayResult()
        {
            displayOutput.Text = label;
        }
    }
}
