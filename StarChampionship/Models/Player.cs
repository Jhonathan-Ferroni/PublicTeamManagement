using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace StarChampionship.Models
{
    public class Player
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "{0} required")]
        [StringLength(60, MinimumLength = 3, ErrorMessage = "{0} size should be between {2} and {1}")]
        public string Name { get; set; }

        [Display(Name = "Imagem (URL)")]
        public string? ImageUrl { get; set; }

        [Range(0, 100)]
        public double Shoot { get; set; }

        [Range(0, 100)]
        public double Dribble { get; set; }

        [Range(0, 100)]
        public double FirstTouch { get; set; }

        [Range(0, 100)]
        public double BallControl { get; set; }

        [Range(0, 100)]
        public double Defense { get; set; }

        [Range(0, 100)]
        public double Pass { get; set; }

        [Range(0, 100)]
        public double Speed { get; set; }

        [Range(0, 100)]
        public double Strength { get; set; }

        public double Overall => Math.Round((Shoot + Dribble + FirstTouch + BallControl + Defense + Pass + Speed + Strength) / 8, 2);

        public Player()
        {
        }

        // Construtor atualizado incluindo ImageUrl
        public Player(int id, string name, string? imageUrl, double shoot, double dribble, double firstTouch, double ballControl, double defense, double pass, double speed, double strength)
        {
            Id = id;
            Name = name;
            ImageUrl = imageUrl;
            Shoot = shoot;
            Dribble = dribble;
            FirstTouch = firstTouch;
            BallControl = ballControl;
            Defense = defense;
            Pass = pass;
            Speed = speed;
            Strength = strength;
        }
    }
}