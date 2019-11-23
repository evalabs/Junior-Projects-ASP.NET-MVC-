using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using JuniorProjectsTest.Models;
using JuniorProjectsTest.Models.Dtos;

namespace JuniorProjectsTest.Controllers
{
    public class MoviesController : Controller
    {
        private MovieDBContext db = new MovieDBContext();

        // GET: Movies
        public ActionResult Index()
        {
            var userName = Request.IsAuthenticated ? User.Identity.Name : null;

            var page = Request["page"] != null ? Convert.ToInt32(Request["page"]) : 1;
            var limit = 5;
            var offset = (page - 1) * limit;
            var items = db.Movies.OrderByDescending(m => m.ID).Skip(offset).Take(limit).ToList();

            var moviesCount = db.Movies.Count();
            var pagesCount = moviesCount / limit;
            if (moviesCount % limit > 0)
                pagesCount++;

            items = items == null || items.Count == 0 ? new List<Movie> { new Movie() } : items;

            return View(new MovieListDto {
                CurrentUser = userName,
                PagesCount = pagesCount,
                CurrentPage = page,
                Items = items
            });
        }

        // GET: Movies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        [Authorize]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Movies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create([Bind(Include = "ID,Title,Description,ReleaseDate,Director,UserName,ImageSrc")] Movie movie)
        {
            if (ModelState.IsValid)
            {
                var userName = User.Identity.Name;
                movie.UserName = userName;
                db.Movies.Add(movie);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(movie);
        }

        // GET: Movies/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            var userName = User.Identity.Name;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            if(movie.UserName != userName)
            {
                return View("NotOwner");
            }
            return View(movie);
        }

        // POST: Movies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit([Bind(Include = "ID,Title,Description,ReleaseDate,Director,UserName,ImageSrc")] Movie movie)
        {
            var userName = User.Identity.Name;
            if (ModelState.IsValid)
            {
                var dbmovie = db.Movies.Find(movie.ID);

                if (dbmovie.UserName != userName)
                {
                    return View("NotOwner");
                }

                dbmovie.Title = movie.Title;
                dbmovie.ReleaseDate = movie.ReleaseDate;
                dbmovie.Description = movie.Description;
                dbmovie.Director = movie.Director;

                db.Entry(dbmovie).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize]
        public ActionResult Delete(int? id)
        {
            var userName = User.Identity.Name;
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Movie movie = db.Movies.Find(id);
            if (movie == null)
            {
                return HttpNotFound();
            }
            if (movie.UserName != userName)
            {
                return View("NotOwner");
            }
            return View(movie);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult DeleteConfirmed(int id)
        {
            var userName = User.Identity.Name;

            Movie movie = db.Movies.Find(id);

            if (movie.UserName != userName)
            {
                return View("NotOwner");
            }

            var oldImageSrc = movie.ImageSrc;
            db.Movies.Remove(movie);
            db.SaveChanges();

            if (oldImageSrc != null)
            {
                var oldPath = Path.Combine(Server.MapPath("~/Content/Images"), oldImageSrc);
                if (System.IO.File.Exists(oldPath))
                    System.IO.File.Delete(oldPath);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize]
        public ActionResult FileUpload(HttpPostedFileBase file)
        {
            var userName = User.Identity.Name;
            var movieId = Convert.ToInt32(Request["MovieID"]);

            if (file != null)
            {
                Movie movie = db.Movies.Find(movieId);
                if (movie == null)
                {
                    return HttpNotFound();
                }
                if (movie.UserName != userName)
                {
                    return View("NotOwner");
                }

                var fileExt = Path.GetExtension(file.FileName);
                var fileName = Guid.NewGuid().ToString();
                // string pic = Path.GetFileName(file.FileName);
                var pic = fileName + fileExt;
                var path = Path.Combine(Server.MapPath("~/Content/Images"), pic);

                file.SaveAs(path);

                using (MemoryStream ms = new MemoryStream())
                {
                    file.InputStream.CopyTo(ms);
                    byte[] array = ms.GetBuffer();
                }

                var oldImageSrc = movie.ImageSrc;
                movie.ImageSrc = pic;
                db.Entry(movie).State = EntityState.Modified;
                db.SaveChanges();

                if (oldImageSrc != null)
                {
                    var oldPath = Path.Combine(Server.MapPath("~/Content/Images"), oldImageSrc);
                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);
                }
            }

            return RedirectToAction("Details", new { id = movieId });
        }

        public ActionResult NotOwner()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
