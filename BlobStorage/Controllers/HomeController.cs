using BlobStorage.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//los 3 using necesarios
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace BlobStorage.Controllers
{
    public class HomeController : Controller
    {
        List<Imagen> imglist = new List<Imagen>();
        public ActionResult Index(String nombre)
        {
            String keys = CloudConfigurationManager.GetSetting("ConexionBlobs");
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            CloudBlobContainer container = blobClient.GetContainerReference("imagenes");
            if (nombre != null)
            {
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(nombre);
                blockBlob.Delete();
            }
            foreach (IListBlobItem item in container.ListBlobs(null, true))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    Imagen img = new Imagen();
                    img.Nombre = blob.Name;
                    img.UrlImagen = blob.Uri.AbsoluteUri;
                    imglist.Add(img);
                }
            }
            return View(imglist);
        }

        [HttpPost]
        public ActionResult Index()
        {
            var file = Request.Files[0];
            if (file != null && file.ContentLength > 0)
            {
                String keys = CloudConfigurationManager.GetSetting("ConexionBlobs");
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(keys);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobClient.GetContainerReference("imagenes");
                CloudBlockBlob blockBlob = container.GetBlockBlobReference(file.FileName);
                blockBlob.UploadFromStream(file.InputStream);
            }
            return RedirectToAction("Index");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}