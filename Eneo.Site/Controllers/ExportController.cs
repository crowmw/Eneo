using Eneo.Model;
using Eneo.Model.Models.Entities;
using Eneo.Model.Models.SerializeModels;
using Eneo.Model.Repository;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Serialization;

namespace Eneo.Site.Controllers
{
    public class ExportController : Controller
    {
        private const string EneoUsers = "EneoUsers";
        private const string AspNetUsers = "AspNetUsers";
        private const string PlacedItems = "PlacedItems";
        private const string DMPDeviceFile = "DMPDeviceInfos";
        private const string DMPFacebookFile = "DMPFacebook";
        private const string DMPPositionFile = "DMPPositions";
        private EneoContext _eneoCtx;
        private ExportRepo _exportRepo;
        private string _xmlPath;

        public ExportController()
        {
            _eneoCtx = new EneoContext();
            _exportRepo = new ExportRepo(_eneoCtx);
        }

        protected override void Initialize(RequestContext requestContext)
        {
            base.Initialize(requestContext);
            string rootServerPath = Directory.GetParent(HttpContext.Server.MapPath("//")).Parent.FullName;
            _xmlPath = Path.Combine(rootServerPath, @"Export\");
            if (!Directory.Exists(_xmlPath))
            {
                Directory.CreateDirectory(_xmlPath);
            }
        }

        public ActionResult Index()
        {
            string[] directories = GetXMLFilenames();
            return View(directories);
        }

        private string[] GetXMLFilenames()
        {
            string[] directories = Directory.GetFiles(_xmlPath);

            for (int i = 0; i < directories.Length; i++)
            {
                directories[i] = Path.GetFileName(directories[i]);
            }
            return directories;
        }

        [HttpPost]
        public ActionResult Users()
        {
            var aspNetUsers = _exportRepo.GetAspNetUsers();
            var eneoUsers = _exportRepo.GetEneoUsers();

            SerializeData(aspNetUsers, GenerateFileName(AspNetUsers));
            SerializeData(eneoUsers, GenerateFileName(EneoUsers));

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult Places()
        {
            var items = _exportRepo.GetCollectionItems();
            var places = _exportRepo.GetPlacedItems();

            try
            {
                // SerializeData(items, "CollectionItems");
                SerializeData(places, GenerateFileName(PlacedItems));
            }
            catch (Exception)
            {
                return new HttpStatusCodeResult(HttpStatusCode.NotFound);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult DMP()
        {
            var infos = _exportRepo.GetDMPDeviceInfo();
            var fbData = _exportRepo.GetDMPFacebook();
            var positions = _exportRepo.GetDMPUserPosition();
            SerializeData(infos, GenerateFileName(DMPDeviceFile));
            SerializeData(positions, GenerateFileName(DMPPositionFile));
            SerializeData(fbData, GenerateFileName(DMPFacebookFile));
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult DMPFacebook()
        {
            var fbData = _exportRepo.GetDMPFacebook();
            SerializeData(fbData, GenerateFileName(DMPFacebookFile));
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult DMPUserPosition()
        {
            var positions = _exportRepo.GetDMPUserPosition();
            SerializeData(positions, GenerateFileName(DMPPositionFile));
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        public ActionResult GetXML(string filename)
        {
            string path = new Uri(_xmlPath + filename).LocalPath;
            ControllerContext.HttpContext.Response.AddHeader("content-disposition", "attachment; filename=" + filename);
            ControllerContext.HttpContext.Response.TransmitFile(path);

            return new EmptyResult();
        }

        public ActionResult Import(string filename)
        {
            string path = _xmlPath + filename;

            try
            {
                string type = filename.Substring(0, filename.IndexOf('.'));
                switch (type)
                {
                    case EneoUsers:
                        var users = DeserializeData<EneoUser>(filename);
                        _exportRepo.AddEneoUsers(users);
                        break;
                    case AspNetUsers:
                        var aspNetUsers = DeserializeData<IdentityUserSM>(filename);
                        _exportRepo.AddAspNetUsers(aspNetUsers);
                        break;
                    case PlacedItems:
                        var placedItems = DeserializeData<PlacedItem>(filename);
                        _exportRepo.AddPlacedItems(placedItems);
                        break;
                    default:
                        break;

                }
            }
            catch (Exception)
            {
                return View("Imported", null, "Error during import");
            }
            return View("Imported", null, "Import successfull!");
        }

        public ActionResult Imported()
        {
            return View();
        }

        public ActionResult Delete(string filename)
        {
            string path = _xmlPath + filename;
            try
            {
                System.IO.File.Delete(path);
            }
            catch (Exception)
            {
                ModelState.AddModelError("DeleteError", "Cannot delete file" + filename);
            }

            string[] xmls = GetXMLFilenames();
            return View("Index", xmls);
        }

        public ActionResult ImportUsers()
        {
            //get two xmls
            //import

            return View();

        }


        public ActionResult bobi()
        {
            var s = _eneoCtx.DMPFacebook.Select(x => x.DMPFacebookLikes).ToList();
            System.Diagnostics.Debug.Write(s.First().First().LikeName);
            return null;
        }

        private string GenerateFileName(string objectName)
        {
            return String.Format("{0}.{1}.{2}.xml", objectName, DateTime.Now.ToString(@"yyyy\.dd\.mm"), DateTime.Now.TimeOfDay.ToString(@"hh\.mm\.ss"));
        }

        private void SerializeData<T>(T model, string filename) where T : class
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (TextWriter writer = new StreamWriter(_xmlPath + filename))
            {
                serializer.Serialize(writer, model);
                writer.Close();
            }
        }

        private List<T> DeserializeData<T>(string filename) where T : class
        {
            List<T> model;
            XmlSerializer serializer = new XmlSerializer(typeof(List<T>));
            using (TextReader reader = new StreamReader(_xmlPath + filename))
            {
                model = serializer.Deserialize(reader) as List<T>;
            }
            return model;
        }
    }
}