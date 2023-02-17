﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string ISBN { get; set; }

        [Required]
        public string Author { get; set; }

        [Required, Display(Name = "List price")]
        [Range(1, 10000)]
        public double ListPrice { get; set; }

        [Required, Display(Name = "Price for 1-50")]
        [Range(1, 10000)]
        public double Price { get; set; }

        [Required, Display(Name = "Price for 51-100")]
        [Range(1, 10000)]
        public double Price50 { get; set; }

        [Required, Display(Name = "Price for 100+")]
        [Range(1, 10000)]
        public double Price100 { get; set; }

        [ValidateNever]
        public string ImageUrl { get; set; }

        [Required, Display(Name ="Category")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId"), ValidateNever]
        public Category Category { get; set; }

        [Required, Display(Name ="Cover type")]
        public int CoverTypeId { get; set; }
        [ValidateNever]
        public CoverType CoverType { get; set; }
    }
}