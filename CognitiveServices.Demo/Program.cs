using CognitiveServices.FaceApi;
using CognitiveServices.FaceApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace CognitiveServices.Demo
{
    class Program
    {

        const string subscriptionKey = "5dabb79d306d40e89ae25c74d43dbba0";
        const string path = "mr-bean.jpg";
        const string uriBase = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect";

        static void Main(string[] args)
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            var faceInfoList = await GetFaceInformation();
            var firstFace = faceInfoList.FirstOrDefault();
            if (firstFace == null)
            {
                return;
            }
            else
            {
                ModofyImage(firstFace);
            }
        }

        private static void ModofyImage(FaceInformation faceInformation)
        {
            var jLoImage = Image.FromFile("Images\\jlo.jpg");
            var glassesImage = Image.FromFile("Images\\glasses.png");
            var graphics = Graphics.FromImage(jLoImage);

            var from = faceInformation.faceLandmarks.eyeLeftOuter;
            var to = faceInformation.faceLandmarks.eyeRightOuter;

            var width = jLoImage.Width;
            var height = jLoImage.Height;

            Graphics graphicsMrBean = Graphics.FromImage(jLoImage);

            //graphics.DrawLine(Pens.Black, new Point(0, 0),
            //    new Point(width, height));

            var glassesWidth = to.x - from.x;
            var glassesHeight =
faceInformation.faceLandmarks.eyeLeftBottom.y - faceInformation.faceLandmarks.eyeLeftTop.y;

            glassesWidth *= 1.2f;
            glassesHeight *= 1.2f;
            var glassesWidthInPixels = Convert.ToInt32(Math.Floor(glassesWidth));
            var glassesHeightInPixels = Convert.ToInt32(Math.Floor(glassesHeight));

            graphicsMrBean.DrawImage(glassesImage, (int)from.x, (int)from.y
                , glassesWidthInPixels, glassesHeightInPixels);

            graphicsMrBean.Save();
            jLoImage.Save("new.png");
        }

        public static string GetFaceInfo()
        {
            var bytes = GetImageBytes();
            var base64string = Convert.ToBase64String(bytes);

            return base64string;
        }

        public static byte[] GetImageBytes()
        {
            var fileBytes = File.ReadAllBytes("Images\\jlo.jpg");
            return fileBytes;
        }

        static async Task<List<FaceInformation>> GetFaceInformation()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", subscriptionKey);
            string requestParameters = "returnFaceId=true&returnFaceLandmarks=true&returnFaceAttributes=age,gender,headPose,smile,facialHair,glasses,emotion,hair,makeup,occlusion,accessories,blur,exposure,noise";
            string uri = uriBase + "?" + requestParameters;
            HttpResponseMessage response;
            byte[] byteData = GetImageBytes();
            using (ByteArrayContent content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
                string contentString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<FaceInformation>>(contentString);
            }
        }
    }
}