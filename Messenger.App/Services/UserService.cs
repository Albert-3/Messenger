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
        private readonly IJwtTokenGenerator _tokenGenerator;
        public UserService(IUserRepository userRepository,
            IValidator<RegistrationDTO> validator,
            IValidator<LoginDTO> validatorLog,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator tokenGenerator)
        {
            _validatorReg = validator;
            _userRepository = userRepository;
            _validatorLog = validatorLog;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
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
                throw new Exception("Invalid login attempt. Please check your credentials.");
            }
            var hasherPass = _passwordHasher.Verify(loginDTO.Password, user.Password);
            var token = _tokenGenerator.GenerateToken(user);
            if (user != null && hasherPass)
            {
                return user;
            }
            throw new Exception("Invalid login attempt. Please check your credentials.");
        }
        public async Task<List<GetUsersDTO>> GetUsers(Guid resipentId)
        {
            var userData = await _userRepository.GetUsers(resipentId);
            return userData
                .Select(user => new GetUsersDTO
                {
                    UserName = user.UserName,
                    Id = user.Id
                }).ToList();
        }
        public async Task<GetUsersDTO> GetUserById(Guid guid)
        {

            var users = new List<GetUsersDTO>();

            var userData = await _userRepository.GetUserById(guid);

            var userDTO = new GetUsersDTO
            {
                UserName = userData.UserName,
                Id = userData.Id,
            };
            return userDTO;
        }
    }
}
