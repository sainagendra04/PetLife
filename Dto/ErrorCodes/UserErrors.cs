namespace PetLife.Dto.ErrorCodes
{
    public class UserErrors
    {
        public string UserNotFoundError = "User with the specified ID does not exist.";
        public string UsernameNotFound = "User with the specified Username does not exist.";
        public string EmailNotFound = "User with the specified Email does not exist.";
        public string UserPasswordMismatch = "The provided password does not match our records.";
        public string UserWithUserIdExist = "User with specified ID exists";
        public string UserWithEmailExists = "User with the specified Emial exist";
        public string UserWithNameOrEmailExists = "User with the specified Username or Email exist";
        public string InvalidUserCredentials = "Invalid username or password";
    }
}
