using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;

namespace UserSignUpAPI.Controllers
{
    [Route("api/user/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IEmailService _emailService;

        public UserController(DataContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }



        [HttpPost(Name = "register")]
        public async Task<IActionResult> Register(UserRegisterRequest request)
        {
            if (_context.Users.Any(u => u.Email == request.Email))
                return BadRequest("User already exists!.");

            CreatePasswordHash(request.Password,
                out byte[] passwordHash, out byte[] passwordSalt);

            var user = new User { Email = request.Email, PasswordHash = passwordHash, PasswordSalt = passwordSalt, VerificationToken = CreateRandomToken() };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok("User Successfully created");
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512();
            passwordSalt = hmac.Key;
            passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

        }

        private string CreateRandomToken()
        {
            return Convert.ToHexString(RandomNumberGenerator.GetBytes(64));
        }


        //Example Value 
        //{
        //  "email": "user@example.com",
        //  "password": "string"
        //}

        [HttpPost(Name = "login")]
        public async Task<IActionResult> Login(UserLoginRequest requestLogin)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == requestLogin.Email);
            if (user is null) return BadRequest("User not found");
            if (!VerifyPasswordHash(requestLogin.Password, user.PasswordHash, user.PasswordSalt)) return BadRequest("User credentials not valid.");
            if (user.VerifiedAt is null) return BadRequest("Not verified");
            return Ok($"Welcome back, {user.Email} ! , :) ");
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using var hmac = new HMACSHA512(passwordSalt);
            var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

            return computedHash.SequenceEqual(passwordHash);
        }


        [HttpPost(Name = "verify")]
        public async Task<IActionResult> Verify(string token)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.VerificationToken == token);
            if (user is null) return BadRequest("Invalid Token");

            user.VerifiedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return Ok($"User verified! ");
        }


        [HttpPost(Name = "forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user is null) return BadRequest("User not found");

            user.PasswordResetToken = CreateRandomToken();
            user.ResetTokenExpires = DateTime.Now.AddDays(1);
            await _context.SaveChangesAsync();

            // TODO: Send Email

            return Ok($"Yoy may now reset your password.");
        }


        [HttpPost(Name = "reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.PasswordResetToken == request.Token);
            if (user is null || user.ResetTokenExpires > DateTime.Now) return BadRequest(" Invalid token");

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            user.PasswordResetToken = null;
            user.ResetTokenExpires = null;

            await _context.SaveChangesAsync();

            // TODO: Send Email

            return Ok($"Password successfully reset.");
        }



        // To create a email fake use : https://ethereal.email
        // Install MailKit

        [HttpPost(Name = "send-email")]
        public IActionResult SendEmail(EmailDTO request)
        {
            _emailService.SendEmail(request);
            return Ok();
        }


    }
}
