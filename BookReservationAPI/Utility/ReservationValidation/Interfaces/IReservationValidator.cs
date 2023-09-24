﻿using BookReservationAPI.Models;
using BookReservationAPI.Models.Dto;
using BookReservationAPI.Repository.Interfaces;

namespace BookReservationAPI.Utility.ReservationValidation.Interfaces
{
    public interface IReservationValidator
    {
        Task<ReservationValidatorResult> Validate(IUnitOfWork unitOfWork, ReservationCreateDto reservationCreateDto);
    }
}
