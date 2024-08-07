export interface ReservationDto {
  id: number;
  reservationDate: Date;
  reservationEnd: Date;
  pickupDate: Date | null;
  returnDate: Date | null;
  bookTitle: string;
  userUsername: string;
  copyBarcode: string;
}
