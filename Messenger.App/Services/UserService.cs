using FluentValidation;
using FluentValidation.Results;
using Messenger.App.DTOs;
using Messenger.Domain;
using Messenger.Domain.Interface;
using Messenger.Domain.Interfaces;

namespace Messenger.App.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<RegistrationDTO> _validatorReg;
        private readonly IValidator<LoginDTO> _validatorLog;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IValidator<RegistrationDTO> validator, IValidator<LoginDTO> validatorLog, IPasswordHasher passwordHasher)
        {
            _validatorReg = validator;
            _userRepository = userRepository;
            _validatorLog = validatorLog;
            _passwordHasher = passwordHasher;
        }
        public async Task<ValidationResult> Register(RegistrationDTO registrationDTO)
        {
            var validationResult = _validatorReg.Validate(registrationDTO);
            var hashedPassword = _passwordHasher.Generate(registrationDTO.Password);
            if (!validationResult.IsValid)
            {
                return validationResult;
            }

            if ((await _userRepository.CheckUserExist(registrationDTO.UserName)))
            {
                validationResult.Errors.Add(new("UserName", "User already exists."));
                return validationResult;
            }

            var user = new User
            {
                UserName = registrationDTO.UserName,
                Password = hashedPassword,
                PhoneNuber = registrationDTO.PhoneNumber,
            };

            await _userRepository.CreateAsync(user);

            return validationResult;
        }
        public async Task<User> Login(LoginDTO loginDTO)
        {
            var validationResult = _validatorLog.Validate(loginDTO);
            var user = await _userRepository.GetByUser(loginDTO.UserName);
            if (!validationResult.IsValid)
            {
                return null;
            }
            var hasherPass = _passwordHasher.Verify(loginDTO.Password, user.Password);
            if (user != null && hasherPass)
                return user;
            return null;
        }
        public async Task<List<GetUsersDTO>> GetUsers(Guid resipentId)
        {
            var users = new List<GetUsersDTO>();
            var userData = await _userRepository.GetUsers(resipentId);
            foreach (var user in userData)
            {
                var userDTO = new GetUsersDTO
                {
                    UserName = user.UserName,
                    Id = user.Id,
                };
                users.Add(userDTO);
            }
            return users;
        }
    }
}
