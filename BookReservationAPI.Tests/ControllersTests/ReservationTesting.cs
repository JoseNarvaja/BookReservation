using AutoMapper;
using BookReservationAPI.Controllers;
using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;
using BookReservationAPI.Tests.Jwt;
using BookReservationAPI.Utility;
using BookReservationAPI.Utility.ReservationValidation;
using BookReservationAPI.Utility.ReservationValidation.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Moq;
using System.Linq.Expressions;
using System.Net;

namespace BookReservationAPI.Tests.Controllers
{
    public class ReservationTesting : EndToEndTestCase
    {
    }
}
