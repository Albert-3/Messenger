using FluentValidation;
using FluentValidation.Results;
using Messenger.App.DTOs;
using Messenger.Domain;
using Messenger.Domain.Interface;
using Messenger.Domain.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Messenger.App.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<RegistrationDTO> _validatorReg;
        private readonly IValidator<LoginDTO> _validatorLog;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _tokenGenerator;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;
        public UserService(IUserRepository userRepository,
            IValidator<RegistrationDTO> validator,
            IValidator<LoginDTO> validatorLog,
            IPasswordHasher passwordHasher,
            IJwtTokenGenerator tokenGenerator,
            IMemoryCache cache,
            IEmailService emailService)
        {
            _validatorReg = validator;
            _userRepository = userRepository;
            _validatorLog = validatorLog;
            _passwordHasher = passwordHasher;
            _tokenGenerator = tokenGenerator;
            _cache = cache;
            _emailService = emailService;
        }
        public async Task<ValidationResult> Register(RegistrationDTO registrationDTO)
        {

            var validationResult = _validatorReg.Validate(registrationDTO);
            if (!validationResult.IsValid)
            {
                return validationResult;
            }
            return validationResult;
        }
        public async Task<User> Login(LoginDTO loginDTO)
        {

            var validationResult = _validatorLog.Validate(loginDTO);
            if (!validationResult.IsValid)
            {
                return null;
            }
            var user = await _userRepository.GetByUser(loginDTO.UserName);
            if (user == null)
            {
                return null;
            }
            var hasherPass = _passwordHasher.Verify(loginDTO.Password, user.Password);
            if (!hasherPass)
            {
                return null;
            }

            if (_cache.TryGetValue($"Token_{user.UserName}", out string cachedToken))
            {
                return null;
            }

            return user;
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
        public async Task<bool> SendConfirmationEmailAsync(RegistrationDTO registrationDto, string token ,CancellationToken cancellation)
        {
            string confirmLink = $"https://localhost:7275/ConfirmEmail?token={token}";
            string subject = "Confirm Your ChutHub Account Registration";

            string body = $@"
        <div style='font-family: Arial, sans-serif; font-size: 16px; color: #333;'>
            <h2 style='color: #3f71cc;'>Hello, {registrationDto.UserName}!</h2>
            <p>Thank you for joining <strong>ChutHub</strong>, your friendly online chat platform.</p>
            <p>To complete your registration, please confirm your email address by clicking the button below:</p>
            <p style='text-align: center; margin: 30px 0;'>
                <a href='{confirmLink}' style='background-color: #3f71cc; color: white; padding: 12px 24px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                    Confirm Email
                </a>
            </p>
            <p>If you did not sign up for ChutHub, please feel free to ignore this email.</p>
            <p>We’re excited to have you with us!</p>
            <hr style='border: none; border-top: 1px solid #ddd; margin: 40px 0;' />
            <p style='font-size: 14px; color: #777;'>Best regards,<br/>The ChutHub Team</p>
        </div>";

            await _emailService.SendEmailAsync(registrationDto.Email, subject, body , cancellation);
            return true;
        }


        public async Task<bool> ConfirmEmailAsync(string token)
        {
            if (_cache.TryGetValue(token, out RegistrationDTO registrationDTO))
            {
                var hashedPassword = _passwordHasher.Generate(registrationDTO.Password);
                var user = new User
                {
                    UserName = registrationDTO.UserName,
                    Password = hashedPassword,
                    PhoneNuber = registrationDTO.PhoneNumber,
                    Email = registrationDTO.Email,
                };

                await _userRepository.CreateAsync(user);
                _cache.Remove(token);
                _cache.Remove($"Token_{registrationDTO.UserName}"); // Հիմա այս key-ը պետք չէ

                return true;
            }

            return false;
        }
    }
}
