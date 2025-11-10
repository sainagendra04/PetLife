namespace PetLife.Dto
{
    public class RegisterCustomerDto
    {
        //User Details
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        //Customer Details
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
