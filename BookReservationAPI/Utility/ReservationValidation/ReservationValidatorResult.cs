namespace BookReservationAPI.Utility.ReservationValidation
{
    public class ReservationValidatorResult
    {
        public bool success { get; set; }
        public List<string> Errors { get; set; }

        public ReservationValidatorResult()
        {
            Errors = new List<string>();
        }
    }
}
