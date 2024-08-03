export interface ReservationDto {
  id: number;
  reservationDate: Date;
  reservationEnd: Date;
  pickupDate: Date | null;
  returnDate: Date | null;
  bookId: number;
  userId: string;
  copyId: number;
}
