using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace JuniorProjectsTest.Models
{
    public class Movie
    {
        public int ID { get; set; }

        [Display(Name = "Название")]
        [StringLength(60, MinimumLength = 3)]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        [StringLength(600, MinimumLength = 3)]
        public string Description { get; set; }

        [Display(Name = "Год Выпуска")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReleaseDate { get; set; }

        [Display(Name = "Режиссёр")]
        [StringLength(60, MinimumLength = 3)]
        public string Director { get; set; }

        [Display(Name = "Владелец")]
        public string UserName { get; set; }

        public string ImageSrc { get; set; }
    }
    public class MovieDBContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }
    }
}